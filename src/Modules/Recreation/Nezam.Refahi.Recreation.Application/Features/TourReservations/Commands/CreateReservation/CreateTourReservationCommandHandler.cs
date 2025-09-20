using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Recreation.Application.Services;
using Nezam.Refahi.Shared.Application.Common.Interfaces;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.CreateReservation;

public class CreateTourReservationCommandHandler
    : IRequestHandler<CreateTourReservationCommand, ApplicationResult<CreateTourReservationResponse>>
{
    private readonly ITourRepository _tourRepository;
    private readonly ITourReservationRepository _reservationRepository;
    private readonly IRecreationUnitOfWork _unitOfWork;
    private readonly IMemberService _memberService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ParticipantValidationService _participantValidationService;
    private readonly ILogger<CreateTourReservationCommandHandler> _logger;

    public CreateTourReservationCommandHandler(
        ITourRepository tourRepository,
        ITourReservationRepository reservationRepository,
        IRecreationUnitOfWork unitOfWork,
        IMemberService memberService,
        ICurrentUserService currentUserService,
        ParticipantValidationService participantValidationService,
        ILogger<CreateTourReservationCommandHandler> logger)
    {
        _tourRepository = tourRepository;
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
        _memberService = memberService;
        _logger = logger;
        _currentUserService = currentUserService;
        _participantValidationService = participantValidationService;
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
            // Check tour capacity before proceeding
            var currentTotalParticipants = tour.GetConfirmedReservationCount() + tour.GetPendingReservationCount();
            var requestedParticipants = 1 + request.Guests.Count(); // 1 for main participant + guests
            
            if (currentTotalParticipants + requestedParticipants > tour.MaxParticipants)
            {
                return ApplicationResult<CreateTourReservationResponse>.Failure($"ظرفیت تور کافی نیست. ظرفیت باقی‌مانده: {tour.MaxParticipants - currentTotalParticipants}");
            }

            // Validate guest count limits (maximum 5 guests per reservation)
            const int maxGuestsPerReservation = 5;
            var guestCount = request.Guests?.Count() ?? 0;
            
            if (guestCount > maxGuestsPerReservation)
            {
                return ApplicationResult<CreateTourReservationResponse>.Failure($"حداکثر {maxGuestsPerReservation} مهمان در هر رزرو مجاز است");
            }

            // Check if tour is too close to start date (e.g., cannot reserve less than 24 hours before)
            var hoursUntilTourStart = (tour.TourStart - DateTime.UtcNow).TotalHours;
            if (hoursUntilTourStart < 24)
            {
                return ApplicationResult<CreateTourReservationResponse>.Failure("امکان رزرو کمتر از 24 ساعت قبل از شروع تور وجود ندارد");
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
            if (!IsValidIranianNationalId(member.NationalCode))
            {
                return ApplicationResult<CreateTourReservationResponse>.Failure("کد ملی شرکت‌کننده اصلی معتبر نیست");
            }

            // Check if member already has a pending or confirmed reservation for this tour
            var existingReservations = await _reservationRepository.GetByTourIdAndNationalNumberAsync(
                request.TourId, member.NationalCode, cancellationToken:cancellationToken);

            if (existingReservations.Any(r => r.Status == ReservationStatus.Held || r.Status == ReservationStatus.Confirmed))
            {
                return ApplicationResult<CreateTourReservationResponse>.Failure("شما قبلاً برای این تور رزرو انجام داده‌اید");
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

            // Create reservation
            var expiryDate = DateTime.UtcNow.AddMinutes(15);
            var trackingCode = TourReservation.GenerateTrackingCode();
            var 
              reservation = new TourReservation(
                tourId: request.TourId,
                trackingCode: trackingCode,
                capacityId: request.CapacityId != Guid.Empty ? request.CapacityId : null,
                memberId: memebr.Id,
                expiryDate: expiryDate,
                notes: request.Notes);

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

            return ApplicationResult<CreateTourReservationResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating tour reservation for tour {TourId}",
                request.TourId);
            return ApplicationResult<CreateTourReservationResponse>.Failure("خطا در ایجاد رزرو تور");
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

        var activeReservations = existingReservations.Where(r =>
            r.Status == ReservationStatus.Held ||
            r.Status == ReservationStatus.Confirmed).ToList();

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
                        var capabilityName = GetCapabilityDisplayName(requiredCapability);
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
                        var featureName = GetFeatureDisplayName(requiredFeature);
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

            return ApplicationResult.Failure("خطا در بررسی صلاحیت‌های عضویت. لطفاً مجدداً تلاش کنید");
        }
    }

    private static string GetCapabilityDisplayName(string capabilityId)
    {
        // Map capability IDs to Persian display names
        return capabilityId switch
        {
            "structure_supervisor_grade1" => "ناظر سازه درجه یک",
            "architecture_supervisor_grade1" => "ناظر معماری درجه یک",
            "designer_grade1" => "طراح درجه یک",
            "execute_grade1" => "مجری درجه یک",
            "architecture_execute_grade2" => "مجری معماری درجه دو",
            "عضویت_فعال" => "عضویت فعال",
            "بیمه_معتبر" => "بیمه معتبر",
            "مجوز_سفر" => "مجوز سفر",
            _ => capabilityId
        };
    }

    private static string GetFeatureDisplayName(string featureId)
    {
        // Map feature IDs to Persian display names
        return featureId switch
        {
            "structure" => "سازه",
            "architecture" => "معماری",
            "execute" => "اجرا",
            "grade1" => "درجه یک",
            "grade2" => "درجه دو",
            "grade3" => "درجه سه",
            "supervisor" => "ناظر",
            "designer_grade1" => "طراح درجه یک",
            "علاقه_به_عکاسی" => "علاقه به عکاسی",
            "طبیعت‌دوست" => "طبیعت‌دوست",
            "علاقه_به_تاریخ" => "علاقه به تاریخ",
            "عاشق_معماری" => "عاشق معماری",
            "عاشق_شعر" => "عاشق شعر",
            "علاقه_مند_فرهنگ" => "علاقه‌مند فرهنگ",
            _ => featureId
        };
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
                if (!IsValidIranianNationalId(guest.NationalNumber))
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
                if (!string.IsNullOrWhiteSpace(guest.PhoneNumber) && !IsValidIranianPhoneNumber(guest.PhoneNumber))
                {
                    errors.Add($"شماره تلفن مهمان {guestName} معتبر نیست. باید به فرم 09xxxxxxxxx باشد");
                }

              
                    var age = CalculateAge(guest.BirthDate, tour.TourStart);
                    
                    // Check reasonable birth date (not in future, not too old)
                    if (guest.BirthDate > DateTime.Now.Date)
                    {
                        errors.Add($"تاریخ تولد مهمان {guestName} نمی‌تواند در آینده باشد");
                        continue;
                    }

                    if (age > 120)
                    {
                        errors.Add($"سن مهمان {guestName} غیر منطقی است");
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
              

                // Check if guest already has a reservation for this tour
                var existingGuestReservations = await _reservationRepository.GetByTourIdAndNationalNumberAsync(
                    tour.Id, guest.NationalNumber, cancellationToken);

                if (existingGuestReservations.Any(r => r.Status == ReservationStatus.Held || r.Status == ReservationStatus.Confirmed))
                {
                    errors.Add($"مهمان {guestName} قبلاً برای این تور رزرو دارد");
                }

                // Validate emergency contact if provided
                if (!string.IsNullOrWhiteSpace(guest.EmergencyContactPhone) && 
                    !IsValidIranianPhoneNumber(guest.EmergencyContactPhone))
                {
                    errors.Add($"شماره تماس اضطراری مهمان {guestName} معتبر نیست");
                }

                // Validate email format if provided
                if (!string.IsNullOrWhiteSpace(guest.Email) && !IsValidEmail(guest.Email))
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
            return ApplicationResult.Failure("خطا در اعتبارسنجی اطلاعات مهمانان");
        }
    }

    private static int CalculateAge(DateTime birthDate, DateTime referenceDate)
    {
        var age = referenceDate.Year - birthDate.Year;
        if (referenceDate.Date < birthDate.AddYears(age))
            age--;
        return age;
    }

    /// <summary>
    /// Validates Iranian National ID using the official checksum algorithm
    /// </summary>
    private static bool IsValidIranianNationalId(string nationalId)
    {
        if (string.IsNullOrWhiteSpace(nationalId))
            return false;

        // Remove any spaces or special characters
        nationalId = nationalId.Trim().Replace("-", "").Replace(" ", "");

        // Must be exactly 10 digits
        if (nationalId.Length != 10 || !nationalId.All(char.IsDigit))
            return false;

        // Check for invalid repeated digits (like 0000000000, 1111111111, etc.)
        if (nationalId.All(c => c == nationalId[0]))
            return false;

        // Iranian National ID checksum algorithm
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            sum += (nationalId[i] - '0') * (10 - i);
        }

        int remainder = sum % 11;
        int checkDigit = remainder < 2 ? remainder : 11 - remainder;

        return checkDigit == (nationalId[9] - '0');
    }

    /// <summary>
    /// Validates Iranian phone number format
    /// </summary>
    private static bool IsValidIranianPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        // Remove spaces, dashes, and parentheses
        phoneNumber = phoneNumber.Trim()
            .Replace(" ", "")
            .Replace("-", "")
            .Replace("(", "")
            .Replace(")", "");

        // Support formats: 09xxxxxxxxx, +989xxxxxxxxx, 989xxxxxxxxx
        if (phoneNumber.StartsWith("+98"))
            phoneNumber = phoneNumber.Substring(3);
        else if (phoneNumber.StartsWith("98"))
            phoneNumber = phoneNumber.Substring(2);

        // Must be 11 digits starting with 09
        return phoneNumber.Length == 11 && 
               phoneNumber.StartsWith("09") && 
               phoneNumber.All(char.IsDigit);
    }

    /// <summary>
    /// Validates email format
    /// </summary>
    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email.Trim();
        }
        catch
        {
            return false;
        }
    }
}