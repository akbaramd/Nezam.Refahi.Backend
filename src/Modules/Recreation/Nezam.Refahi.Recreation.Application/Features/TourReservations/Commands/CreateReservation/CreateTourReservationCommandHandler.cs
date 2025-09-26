using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Recreation.Domain.ValueObjects;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Recreation.Application.Services;
using Nezam.Refahi.Recreation.Application.Configuration;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using System.Text.Json;
using Nezam.Refahi.Recreation.Application.Services.Contracts;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.CreateReservation;

public class CreateTourReservationCommandHandler
    : IRequestHandler<CreateTourReservationCommand, ApplicationResult<CreateTourReservationResponse>>
{
    private readonly ITourRepository _tourRepository;
    private readonly ITourReservationRepository _reservationRepository;
    private readonly IApiIdempotencyRepository _idempotencyRepository;
    private readonly IRecreationUnitOfWork _unitOfWork;
    private readonly IMemberService _memberService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ParticipantValidationService _participantValidationService;
    private readonly IValidationService _validationService;
    private readonly IDisplayNameService _displayNameService;
    private readonly ReservationSettings _settings;
    private readonly ILogger<CreateTourReservationCommandHandler> _logger;

    public CreateTourReservationCommandHandler(
        ITourRepository tourRepository,
        ITourReservationRepository reservationRepository,
        IApiIdempotencyRepository idempotencyRepository,
        IRecreationUnitOfWork unitOfWork,
        IMemberService memberService,
        ICurrentUserService currentUserService,
        ParticipantValidationService participantValidationService,
        IValidationService validationService,
        IDisplayNameService displayNameService,
        IOptions<ReservationSettings> settings,
        ILogger<CreateTourReservationCommandHandler> logger)
    {
        _tourRepository = tourRepository;
        _reservationRepository = reservationRepository;
        _idempotencyRepository = idempotencyRepository;
        _unitOfWork = unitOfWork;
        _memberService = memberService;
        _logger = logger;
        _currentUserService = currentUserService;
        _participantValidationService = participantValidationService;
        _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
        _displayNameService = displayNameService ?? throw new ArgumentNullException(nameof(displayNameService));
        _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
    }

    public async Task<ApplicationResult<CreateTourReservationResponse>> Handle(
        CreateTourReservationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
          var userId = _currentUserService.UserId;

          if (!userId.HasValue)
          {
            return ApplicationResult<CreateTourReservationResponse>.Failure("کاربر درخواست کننده یافت نشد");
          }

          // Handle idempotency if IdempotencyKey is provided
          if (!string.IsNullOrEmpty(request.IdempotencyKey))
          {
              var tenantId = _settings.DefaultTenantId;
              var endpoint = "CreateTourReservation";
              
              var existingIdempotency = await _idempotencyRepository.GetByKeyAsync(
                  tenantId, endpoint, request.IdempotencyKey, cancellationToken);
                  
              if (existingIdempotency != null)
              {
                  if (existingIdempotency.IsProcessed && !string.IsNullOrEmpty(existingIdempotency.ResponseData))
                  {
                      // Return cached response
                      var cachedResponse = JsonSerializer.Deserialize<CreateTourReservationResponse>(existingIdempotency.ResponseData);
                      _logger.LogInformation("Returning cached response for idempotency key {IdempotencyKey}", request.IdempotencyKey);
                      return ApplicationResult<CreateTourReservationResponse>.Success(cachedResponse!);
                  }
                  else if (!existingIdempotency.IsExpired)
                  {
                      // Request is still being processed
                      return ApplicationResult<CreateTourReservationResponse>.Failure("درخواست در حال پردازش است. لطفاً چند لحظه صبر کنید");
                  }
              }
          }

          var memebr = await _memberService.GetMemberByExternalIdAsync(userId.Value.ToString());
          if (memebr==null)
          {
            return ApplicationResult<CreateTourReservationResponse>.Failure("عضو یافت نشد");
          }
            _logger.LogInformation("Creating tour reservation for tour {TourId} with main participant {NationalNumber}",
                request.TourId, memebr.NationalCode);

            // Get tour
            var tour = await _tourRepository.FindOneAsync(x=>x.Id == request.TourId, cancellationToken:cancellationToken);
            if (tour == null)
            {
                return ApplicationResult<CreateTourReservationResponse>.Failure("تور مورد نظر یافت نشد");
            }

            // Validate tour is active and registration is open
            if (!tour.IsActive)
            {
                return ApplicationResult<CreateTourReservationResponse>.Failure("تور غیرفعال است");
            }

            var currentDate = DateTime.UtcNow;
            if (!tour.IsRegistrationOpen(currentDate))
            {
                return ApplicationResult<CreateTourReservationResponse>.Failure("زمان ثبت نام برای این تور به پایان رسیده است");
            }
            // Check tour capacity before proceeding using domain behavior
            // This automatically excludes expired reservations
            var requestedParticipants = 1 + request.Guests.Count(); // 1 for main participant + guests
            
            if (!tour.HasAvailableSpotsForReservation() || tour.GetAvailableSpots() < requestedParticipants)
            {
                var availableSpots = tour.GetAvailableSpots();
                return ApplicationResult<CreateTourReservationResponse>.Failure($"ظرفیت تور کافی نیست. ظرفیت باقی‌مانده: {availableSpots}");
            }

            // Validate guest count limits based on tour configuration
            var guestCount = request.Guests?.Count() ?? 0;
            
            if (!tour.CanCreateReservationWithGuests(guestCount))
            {
                var maxGuests = tour.MaxGuestsPerReservation ?? 0;
                return ApplicationResult<CreateTourReservationResponse>.Failure($"حداکثر {maxGuests} مهمان در هر رزرو مجاز است");
            }

            // Check if tour is too close to start date
            var hoursUntilTourStart = (tour.TourStart - DateTime.UtcNow).TotalHours;
            if (hoursUntilTourStart < _settings.MinimumHoursBeforeTour)
            {
                return ApplicationResult<CreateTourReservationResponse>.Failure($"امکان رزرو کمتر از {_settings.MinimumHoursBeforeTour} ساعت قبل از شروع تور وجود ندارد");
            }

            // Validate main participant
            var nationalId = new NationalId(memebr.NationalCode);

            // Check if member exists
            var member = await _memberService.GetBasicMemberInfoAsync(nationalId);
            if (member == null)
            {
                return ApplicationResult<CreateTourReservationResponse>.Failure("عضو با کد ملی وارد شده یافت نشد");
            }

            // Check if member has active membership
            var hasActiveMembership = await _memberService.HasActiveMembershipAsync(nationalId);
            if (!hasActiveMembership)
            {
                return ApplicationResult<CreateTourReservationResponse>.Failure("عضویت فعال برای این کد ملی یافت نشد");
            }

            // Check restricted tours - member cannot have existing reservation in restricted tours
            var restrictedTourValidation = await ValidateRestrictedToursAsync(tour, nationalId, cancellationToken:cancellationToken);
            if (!restrictedTourValidation.IsSuccess)
            {
                return ApplicationResult<CreateTourReservationResponse>.Failure(restrictedTourValidation.Errors);
            }
 
            // Validate member capabilities and features
            var capabilityValidation = await ValidateMemberCapabilitiesAsync(tour, member, cancellationToken:cancellationToken);
            if (!capabilityValidation.IsSuccess)
            {
                return ApplicationResult<CreateTourReservationResponse>.Failure(capabilityValidation.Errors);
            }

            // Validate main participant's national ID format
            if (!_validationService.IsValidNationalId(member.NationalCode))
            {
                return ApplicationResult<CreateTourReservationResponse>.Failure("کد ملی شرکت‌کننده اصلی معتبر نیست");
            }

            // Check if member already has an active reservation for this tour
            var existingReservations = await _reservationRepository.GetByTourIdAndNationalNumberAsync(
                request.TourId, member.NationalCode, cancellationToken:cancellationToken);

            // Use domain behavior to check for active reservations (confirmed or pending, excluding expired)
            if (existingReservations.Any(r => r.IsActive()))
            {
                return ApplicationResult<CreateTourReservationResponse>.Failure("شما قبلاً برای این تور رزرو فعال دارید");
            }

            // Check main participant age restrictions
            if (tour.MinAge.HasValue || tour.MaxAge.HasValue)
            {
                // For main participant, we need to get actual birth date from member service
                // For now, we'll skip this validation for main participant since we don't have birth date
                _logger.LogWarning("Age validation skipped for main participant {NationalCode} - birth date not available", 
                    member.NationalCode);
            }

            // Validate capacity if provided
            TourCapacity? selectedCapacity = null;
            if (request.CapacityId != Guid.Empty)
            {
                selectedCapacity = tour.Capacities.FirstOrDefault(c => c.Id == request.CapacityId);
                if (selectedCapacity == null)
                {
                    return ApplicationResult<CreateTourReservationResponse>.Failure("ظرفیت انتخاب شده یافت نشد");
                }

                if (!selectedCapacity.IsActive)
                {
                    return ApplicationResult<CreateTourReservationResponse>.Failure("ظرفیت انتخاب شده غیرفعال است");
                }

                if (!selectedCapacity.IsRegistrationOpen(currentDate))
                {
                    return ApplicationResult<CreateTourReservationResponse>.Failure("زمان ثبت نام برای این ظرفیت به پایان رسیده است");
                }

                // Validate that requested participants fit in selected capacity
                if (selectedCapacity.MaxParticipants < (1 + guestCount))
                {
                    return ApplicationResult<CreateTourReservationResponse>.Failure($"تعداد شرکت‌کنندگان ({1 + guestCount}) از ظرفیت انتخاب شده ({selectedCapacity.MaxParticipants}) بیشتر است");
                }
            }

            // Create idempotency record if key is provided
            ApiIdempotency? idempotencyRecord = null;
            if (!string.IsNullOrEmpty(request.IdempotencyKey))
            {
                var tenantId = _settings.DefaultTenantId;
                var endpoint = "CreateTourReservation";
                
                idempotencyRecord = new ApiIdempotency(
                    idempotencyKey: request.IdempotencyKey,
                    endpoint: endpoint,
                    requestPayloadHash: JsonSerializer.Serialize(request).GetHashCode().ToString(),
                    tenantId: tenantId,
                    userId: userId.Value.ToString(),
                    ttl: TimeSpan.FromMinutes(_settings.IdempotencyTtlMinutes));
                    
                var (record, isNew) = await _idempotencyRepository.GetOrCreateAsync(idempotencyRecord, cancellationToken);
                if (!isNew)
                {
                    return ApplicationResult<CreateTourReservationResponse>.Failure("درخواست تکراری شناسایی شد");
                }
                idempotencyRecord = record;
            }

            // Create reservation with initial state using State Machine
            var expiryDate = DateTime.UtcNow.AddMinutes(_settings.ReservationHoldMinutes);
            var trackingCode = TourReservation.GenerateTrackingCode();
            var initialStatus = ReservationStatus.Draft; // Start with Draft state
            
            var reservation = new TourReservation(
                tourId: request.TourId,
                trackingCode: trackingCode,
                externalUserId: userId.Value,
                capacityId: request.CapacityId != Guid.Empty ? request.CapacityId : null,
                memberId: memebr.Id,
                expiryDate: expiryDate,
                notes: request.Notes);

            // Transition to Held state using State Machine
            if (ReservationStateMachine.IsValidTransition(initialStatus, ReservationStatus.Held))
            {
                // Note: TourReservation entity might need an UpdateStatus method
                // For now, we'll assume it starts in the correct state
                _logger.LogInformation("Reservation created in Held state - ReservationId: {ReservationId}", reservation.Id);
            }
            else
            {
                _logger.LogError("Invalid state transition from {From} to {To}", initialStatus, ReservationStatus.Held);
                return ApplicationResult<CreateTourReservationResponse>.Failure("خطا در ایجاد رزرو");
            }

            // Get pricing for main participant
            var activePricing = tour.GetActivePricing();
            var mainParticipantPricing = activePricing.FirstOrDefault(p => p.ParticipantType == ParticipantType.Member);

            if (mainParticipantPricing == null)
            {
                return ApplicationResult<CreateTourReservationResponse>.Failure("قیمت‌گذاری برای شرکت‌کننده اصلی یافت نشد");
            }

            var requiredAmount = mainParticipantPricing.GetEffectivePrice();

            // Add main participant
            var mainParticipant = new Participant(
                reservationId: reservation.Id,
                firstName: member.FullName.Split(' ').FirstOrDefault() ?? member.FullName,
                lastName: member.FullName.Split(' ').Skip(1).FirstOrDefault() ?? "",
                nationalNumber: member.NationalCode,
                phoneNumber: member.MembershipNumber ?? "-",
                participantType: ParticipantType.Member,
                        requiredAmount: requiredAmount,
                email: "",
                birthDate: DateTime.UtcNow.AddYears(-20),
                emergencyContactName: "",
                emergencyContactPhone: "",
                notes: "");

            reservation.AddParticipant(mainParticipant);

            // Create price snapshot for main participant
            var mainParticipantSnapshot = new ReservationPriceSnapshot(
                reservationId: reservation.Id,
                participantType: ParticipantType.Member,
                basePrice: requiredAmount,
                finalPrice: requiredAmount, // No discount applied initially
                discountAmount: null,
                discountCode: null,
                discountDescription: null,
                pricingRules: JsonSerializer.Serialize(new 
                {
                    PricingId = mainParticipantPricing.Id,
                    ParticipantType = ParticipantType.Member.ToString(),
                    BasePrice = requiredAmount.AmountRials,
                    AppliedAt = DateTime.UtcNow
                }),
                tenantId: "default");

            // Validate and add guests
            if (request.Guests?.Any() == true)
            {
                // Validate guest restrictions
                var guestValidationResult = await ValidateGuestsAsync(tour, request.Guests, member.NationalCode, cancellationToken);
                if (!guestValidationResult.IsSuccess)
                {
                    return ApplicationResult<CreateTourReservationResponse>.Failure(guestValidationResult.Errors);
                }

                // Validate each guest using the validation service
                var participantValidationResults = await _participantValidationService.ValidateParticipantsAsync(request.Guests, cancellationToken);
                
                for (int i = 0; i < request.Guests.Count(); i++)
                {
                    var guest = request.Guests.ElementAt(i);
                    var validationResult = participantValidationResults[i];
                    
                    if (!validationResult.IsValid)
                    {
                        return ApplicationResult<CreateTourReservationResponse>.Failure(validationResult.ErrorMessage!);
                    }

                    // Get pricing based on participant type (member or guest)
                    var participantPricing = activePricing.FirstOrDefault(p => p.ParticipantType == validationResult.ParticipantType);

                    if (participantPricing == null)
                    {
                        var participantTypeText = validationResult.ParticipantType == ParticipantType.Member ? "عضو" : "مهمان";
                        return ApplicationResult<CreateTourReservationResponse>.Failure($"قیمت‌گذاری برای {participantTypeText} یافت نشد");
                    }

                    var amount = participantPricing.GetEffectivePrice();

                    // Create participant with correct type and pricing
                    var guestParticipant = new Participant(
                        reservationId: reservation.Id,
                        firstName: guest.FirstName,
                        lastName: guest.LastName,
                        nationalNumber: guest.NationalNumber,
                        phoneNumber: guest.PhoneNumber,
                        participantType: validationResult.ParticipantType,
                        requiredAmount: amount,
                        email: guest.Email,
                        birthDate: guest.BirthDate,
                        emergencyContactName: guest.EmergencyContactName,
                        emergencyContactPhone: guest.EmergencyContactPhone,
                        notes: guest.Notes);

                    // Add participant to reservation
                    reservation.AddParticipant(guestParticipant);

                    // Create price snapshot for guest participant
                    var guestSnapshot = new ReservationPriceSnapshot(
                        reservationId: reservation.Id,
                        participantType: validationResult.ParticipantType,
                        basePrice: amount,
                        finalPrice: amount, // No discount applied initially
                        discountAmount: null,
                        discountCode: null,
                        discountDescription: null,
                        pricingRules: JsonSerializer.Serialize(new 
                        {
                            PricingId = participantPricing.Id,
                            ParticipantType = validationResult.ParticipantType.ToString(),
                            BasePrice = amount.AmountRials,
                            AppliedAt = DateTime.UtcNow,
                            GuestInfo = new { guest.FirstName, guest.LastName, guest.NationalNumber }
                        }),
                        tenantId: "default");
                }
            }
            
            // Save to database
            // Add reservation to repository first
            await _reservationRepository.AddAsync(reservation, cancellationToken:cancellationToken);

            // Transition to Held state (this will validate participants exist)
            reservation.Hold();

            // Save reservation
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully created reservation {ReservationId} for tour {TourId}",
                reservation.Id, request.TourId);

            // Calculate estimated total price for all participants
            activePricing = tour.GetActivePricing();
            var memberParticipantPricing = activePricing.FirstOrDefault(p => p.ParticipantType == ParticipantType.Member);
            var guestParticipantPricing = activePricing.FirstOrDefault(p => p.ParticipantType == ParticipantType.Guest);
            
            // Calculate total based on actual participant types
            var memberParticipants = reservation.Participants.Count(p => p.ParticipantType == ParticipantType.Member);
            var guestParticipants = reservation.Participants.Count(p => p.ParticipantType == ParticipantType.Guest);
            
            var memberPrice = memberParticipantPricing?.GetEffectivePrice().AmountRials ?? 0;
            var guestPrice = guestParticipantPricing?.GetEffectivePrice().AmountRials ?? 0;
            var estimatedPrice = (memberPrice * memberParticipants) + (guestPrice * guestParticipants);

            var response = new CreateTourReservationResponse
            {
                ReservationId = reservation.Id,
                TrackingCode = reservation.TrackingCode,
                ReservationDate = reservation.ReservationDate,
                ExpiryDate = reservation.ExpiryDate,
                TourTitle = tour.Title,
                TotalParticipants = reservation.Participants.Count,
                EstimatedTotalPrice = estimatedPrice
            };

            // Complete idempotency record if exists
            if (idempotencyRecord != null)
            {
                try
                {
                    idempotencyRecord.MarkAsProcessed(200, JsonSerializer.Serialize(response));
                    await _idempotencyRepository.UpdateAsync(idempotencyRecord, cancellationToken: cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to update idempotency record {IdempotencyKey}", request.IdempotencyKey);
                    // Don't fail the main operation for idempotency update issues
                }
            }

            return ApplicationResult<CreateTourReservationResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating tour reservation for tour {TourId}",
                request.TourId);
            return ApplicationResult<CreateTourReservationResponse>.Failure(ex, "خطا در ایجاد رزرو تور");
        }
    }

    private async Task<ApplicationResult> ValidateRestrictedToursAsync(
        Tour tour,
        NationalId nationalId,
        CancellationToken cancellationToken)
    {
        if (!tour.TourRestrictedTours.Any())
        {
            return ApplicationResult.Success();
        }

        var restrictedTourIds = tour.TourRestrictedTours.Select(rt => rt.RestrictedTourId).ToList();

        // Check if member has any existing reservations in restricted tours
        var existingReservations = await _reservationRepository.GetByTourIdsAndNationalNumberAsync(
            restrictedTourIds, nationalId.Value, cancellationToken:cancellationToken);

        var activeReservations = existingReservations.Where(r => r.IsActive()).ToList();

        if (activeReservations.Any())
        {
            var restrictedTourTitles = string.Join("، ", activeReservations.Select(r => r.Tour.Title));
            return ApplicationResult.Failure($"شما در تورهای محدودکننده زیر رزرو دارید و نمی‌توانید برای این تور رزرو کنید: {restrictedTourTitles}");
        }

        return ApplicationResult.Success();
    }

    private async Task<ApplicationResult> ValidateMemberCapabilitiesAsync(
        Tour tour,
        BasicMemberInfoDto member,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Validating member capabilities for tour {TourId} and member {NationalCode}",
                tour.Id, member.NationalCode);

            // Get required capabilities and features for the tour
            var requiredCapabilities = tour.MemberCapabilities.Select(mc => mc.CapabilityId).ToList();
            var requiredFeatures = tour.MemberFeatures.Select(mf => mf.FeatureId).ToList();

            if (!requiredCapabilities.Any() && !requiredFeatures.Any())
            {
                _logger.LogDebug("Tour {TourId} has no capability or feature requirements", tour.Id);
                return ApplicationResult.Success();
            }

            var nationalCode = new NationalId(member.NationalCode);
            var errors = new List<string>();

            // Validate required capabilities
            if (requiredCapabilities.Any())
            {
                _logger.LogDebug("Checking {Count} required capabilities for member {NationalCode}",
                    requiredCapabilities.Count, member.NationalCode);

                foreach (var requiredCapability in requiredCapabilities)
                {
                    var hasCapability = await _memberService.HasCapabilityAsync(nationalCode, requiredCapability);
                    if (!hasCapability)
                    {
                        var capabilityName = _displayNameService.GetCapabilityDisplayName(requiredCapability);
                        errors.Add($"عضو فاقد صلاحیت مورد نیاز «{capabilityName}» می‌باشد");

                        _logger.LogWarning("Member {NationalCode} lacks required capability: {CapabilityId}",
                            member.NationalCode, requiredCapability);
                    }
                }
            }

            // Validate required features
            if (requiredFeatures.Any())
            {
                _logger.LogDebug("Checking {Count} required features for member {NationalCode}",
                    requiredFeatures.Count, member.NationalCode);

                foreach (var requiredFeature in requiredFeatures)
                {
                    var hasFeature = await _memberService.HasFeatureAsync(nationalCode, requiredFeature);
                    if (!hasFeature)
                    {
                        var featureName = _displayNameService.GetFeatureDisplayName(requiredFeature);
                        errors.Add($"عضو فاقد ویژگی مورد نیاز «{featureName}» می‌باشد");

                        _logger.LogWarning("Member {NationalCode} lacks required feature: {FeatureId}",
                            member.NationalCode, requiredFeature);
                    }
                }
            }

            if (errors.Any())
            {
                var errorMessage = string.Join("، ", errors);
                _logger.LogWarning("Member {NationalCode} cannot participate in tour {TourId}: {Errors}",
                    member.NationalCode, tour.Id, errorMessage);

                return ApplicationResult.Failure(errors,$"شما نمی‌توانید در این تور شرکت کنید: {errorMessage}");
            }

            _logger.LogInformation("Member {NationalCode} meets all capability and feature requirements for tour {TourId}",
                member.NationalCode, tour.Id);

            return ApplicationResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating member capabilities for tour {TourId} and member {NationalCode}",
                tour.Id, member?.NationalCode);

            return ApplicationResult.Failure(ex, "خطا در بررسی صلاحیت‌های عضویت. لطفاً مجدداً تلاش کنید");
        }
    }



    private async Task<ApplicationResult> ValidateGuestsAsync(
        Tour tour,
        IEnumerable<GuestParticipantDto> guests,
        string mainParticipantNationalCode,
        CancellationToken cancellationToken)
    {
        try
        {
            var errors = new List<string>();
            var nationalNumbers = new HashSet<string> { mainParticipantNationalCode };
            var guestList = guests.ToList();

            _logger.LogDebug("Validating {GuestCount} guests for tour {TourId}", guestList.Count, tour.Id);

            // Check for duplicate national numbers within guests
            foreach (var guest in guestList)
            {
                var guestName = $"{guest.FirstName?.Trim()} {guest.LastName?.Trim()}".Trim();
                
                // Validate required fields
                if (string.IsNullOrWhiteSpace(guest.FirstName))
                {
                    errors.Add($"نام مهمان الزامی است");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(guest.LastName))
                {
                    errors.Add($"نام خانوادگی مهمان {guest.FirstName} الزامی است");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(guest.NationalNumber))
                {
                    errors.Add($"کد ملی مهمان {guestName} الزامی است");
                    continue;
                }

                // Validate Iranian National ID format and checksum
                if (!_validationService.IsValidNationalId(guest.NationalNumber))
                {
                    errors.Add($"کد ملی مهمان {guestName} ({guest.NationalNumber}) معتبر نیست");
                    continue;
                }

                // Check if main participant is trying to add themselves as guest
                if (guest.NationalNumber == mainParticipantNationalCode)
                {
                    errors.Add($"شرکت‌کننده اصلی نمی‌تواند به عنوان مهمان اضافه شود");
                    continue;
                }

                // Check for duplicate national numbers within guests
                if (!nationalNumbers.Add(guest.NationalNumber))
                {
                    errors.Add($"کد ملی {guest.NationalNumber} تکراری است (مهمان {guestName})");
                    continue;
                }

                // Validate phone number format
                if (!string.IsNullOrWhiteSpace(guest.PhoneNumber) && !_validationService.IsValidPhoneNumber(guest.PhoneNumber))
                {
                    errors.Add($"شماره تلفن مهمان {guestName} معتبر نیست. باید به فرم 09xxxxxxxxx باشد");
                }

              
                    var age = _validationService.CalculateAge(guest.BirthDate, tour.TourStart);
                    
                    // Check reasonable birth date (not in future, not too old)
                    if (!_validationService.IsReasonableBirthDate(guest.BirthDate))
                    {
                        if (guest.BirthDate > DateTime.Now.Date)
                        {
                            errors.Add($"تاریخ تولد مهمان {guestName} نمی‌تواند در آینده باشد");
                        }
                        else
                        {
                            errors.Add($"سن مهمان {guestName} غیر منطقی است (بیش از {_settings.MaxReasonableAge} سال)");
                        }
                        continue;
                    }

                    if (tour.MinAge.HasValue && age < tour.MinAge.Value)
                    {
                        errors.Add($"مهمان {guestName} سن کافی ندارد. حداقل سن مجاز: {tour.MinAge} سال (سن فعلی: {age} سال)");
                    }
                    
                    if (tour.MaxAge.HasValue && age > tour.MaxAge.Value)
                    {
                        errors.Add($"مهمان {guestName} بیش از حد سن مجاز دارد. حداکثر سن مجاز: {tour.MaxAge} سال (سن فعلی: {age} سال)");
                    }
              

                // Check if guest already has an active reservation for this tour
                var existingGuestReservations = await _reservationRepository.GetByTourIdAndNationalNumberAsync(
                    tour.Id, guest.NationalNumber, cancellationToken);

                if (existingGuestReservations.Any(r => r.IsActive()))
                {
                    errors.Add($"مهمان {guestName} قبلاً برای این تور رزرو فعال دارد");
                }

                // Validate emergency contact if provided
                if (!string.IsNullOrWhiteSpace(guest.EmergencyContactPhone) && 
                    !_validationService.IsValidPhoneNumber(guest.EmergencyContactPhone))
                {
                    errors.Add($"شماره تماس اضطراری مهمان {guestName} معتبر نیست");
                }

                // Validate email format if provided
                if (!string.IsNullOrWhiteSpace(guest.Email) && !_validationService.IsValidEmail(guest.Email))
                {
                    errors.Add($"ایمیل مهمان {guestName} معتبر نیست");
                }
            }

            if (errors.Any())
            {
                _logger.LogWarning("Guest validation failed for tour {TourId}. Errors: {Errors}", 
                    tour.Id, string.Join("; ", errors));
                return ApplicationResult.Failure(errors);
            }

            _logger.LogDebug("All guests validated successfully for tour {TourId}", tour.Id);
            return ApplicationResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating guests for tour {TourId}", tour.Id);
            return ApplicationResult.Failure(ex, "خطا در اعتبارسنجی اطلاعات مهمانان");
        }
    }




}