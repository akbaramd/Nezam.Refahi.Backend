using System.Collections.Concurrent;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nezam.Refahi.Finance.Contracts.Services;
using Nezam.Refahi.Membership.Contracts.Dtos;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Recreation.Application.Configuration;
using Nezam.Refahi.Recreation.Application.Services;
using Nezam.Refahi.Recreation.Application.Services.Contracts;
using Nezam.Refahi.Recreation.Contracts.IntegrationEvents;
using Nezam.Refahi.Recreation.Contracts.Services;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Recreation.Domain.ValueObjects;
using Nezam.Refahi.Shared.Application;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;
using MassTransit;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.FinalizeReservation;

/// <summary>
/// Handler for finalizing a draft reservation
/// Validates participants, creates pricing snapshots, creates bill, and holds the reservation
/// All operations are performed in a transaction with rollback on failure
/// Uses locking to prevent concurrent finalizations of the same reservation
/// </summary>
public class FinalizeReservationCommandHandler : IRequestHandler<FinalizeReservationCommand, ApplicationResult<FinalizeReservationResponse>>
{
    private readonly ITourReservationRepository _reservationRepository;
    private readonly ITourRepository _tourRepository;
    private readonly IRecreationUnitOfWork _unitOfWork;
    private readonly IMemberService _memberService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IParticipantValidationService _participantValidationService;
    private readonly IValidationService _validationService;
    private readonly IReservationPricingService _reservationPricingService;
    private readonly IBillService _billService;
    private readonly ReservationSettings _settings;
    private readonly ILogger<FinalizeReservationCommandHandler> _logger;
    private readonly IBus _publishEndpoint;

    // Locking mechanism to prevent concurrent finalizations of the same reservation
    // Using SemaphoreSlim for in-process locking (for distributed systems, use Redis/Database locks)
    private static readonly ConcurrentDictionary<Guid, SemaphoreSlim> _reservationLocks = new();

    public FinalizeReservationCommandHandler(
        ITourReservationRepository reservationRepository,
        ITourRepository tourRepository,
        IRecreationUnitOfWork unitOfWork,
        IMemberService memberService,
        ICurrentUserService currentUserService,
        IParticipantValidationService participantValidationService,
        IValidationService validationService,
        IReservationPricingService reservationPricingService,
        IBillService billService,
        IOptions<ReservationSettings> settings,
        ILogger<FinalizeReservationCommandHandler> logger,
        IBus publishEndpoint)
    {
        _reservationRepository = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
        _tourRepository = tourRepository ?? throw new ArgumentNullException(nameof(tourRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        _participantValidationService = participantValidationService ?? throw new ArgumentNullException(nameof(participantValidationService));
        _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
        _reservationPricingService = reservationPricingService ?? throw new ArgumentNullException(nameof(reservationPricingService));
        _billService = billService ?? throw new ArgumentNullException(nameof(billService));
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
    }

    public async Task<ApplicationResult<FinalizeReservationResponse>> Handle(
        FinalizeReservationCommand request,
        CancellationToken cancellationToken)
    {
        // Acquire lock for this reservation to prevent concurrent finalizations
        var semaphore = _reservationLocks.GetOrAdd(request.ReservationId, _ => new SemaphoreSlim(1, 1));

        await semaphore.WaitAsync(cancellationToken);
        try
        {
            _logger.LogInformation("Finalizing reservation - ReservationId: {ReservationId}", request.ReservationId);

            // Begin transaction
            await _unitOfWork.BeginAsync(cancellationToken);

            try
            {
                // Get reservation with all related data (locked for update)
                var reservation = await _reservationRepository.FindOneAsync(
                    x => x.Id == request.ReservationId,
                    cancellationToken);

                if (reservation == null)
                {
                    _logger.LogWarning("Reservation not found - ReservationId: {ReservationId}", request.ReservationId);
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<FinalizeReservationResponse>.Failure("رزرو مورد نظر یافت نشد");
                }

                // Validate reservation can be finalized (domain behavior)
                if (!reservation.CanFinalize(out var canFinalizeError))
                {
                    _logger.LogWarning("Reservation cannot be finalized - ReservationId: {ReservationId}, Error: {Error}",
                        request.ReservationId, canFinalizeError);
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<FinalizeReservationResponse>.Failure(canFinalizeError!);
                }

                // Get tour with capacities and pricing
                var tour = await _tourRepository.FindOneAsync(x => x.Id == reservation.TourId, cancellationToken);
                if (tour == null)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<FinalizeReservationResponse>.Failure("تور مورد نظر یافت نشد");
                }

                // Validate tour is active and registration is open
                if (!tour.IsActive)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<FinalizeReservationResponse>.Failure("تور غیرفعال است");
                }

                var currentDate = DateTime.UtcNow;
                if (!tour.IsRegistrationOpen(currentDate))
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<FinalizeReservationResponse>.Failure("زمان ثبت نام برای این تور به پایان رسیده است");
                }

                // Check if tour is too close to start date
                var hoursUntilTourStart = (tour.TourStart - currentDate).TotalHours;
                if (hoursUntilTourStart < _settings.MinimumHoursBeforeTour)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<FinalizeReservationResponse>.Failure(
                        $"امکان رزرو کمتر از {_settings.MinimumHoursBeforeTour} ساعت قبل از شروع تور وجود ندارد");
                }

                // Get current user
                var userId = _currentUserService.UserId;
                if (!userId.HasValue)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<FinalizeReservationResponse>.Failure("کاربر درخواست کننده یافت نشد");
                }

                // Get member by external user ID
                var memberDto = await _memberService.GetMemberByExternalIdAsync(userId.Value.ToString());
                if (memberDto == null)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<FinalizeReservationResponse>.Failure("عضو یافت نشد");
                }

                // Get detailed member information
                var nationalId = new NationalId(memberDto.NationalCode);
                var memberDetail = await _memberService.GetMemberDetailByNationalCodeAsync(nationalId);
                if (memberDetail == null)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<FinalizeReservationResponse>.Failure("اطلاعات عضو یافت نشد");
                }

                // Validate member has active membership
                var hasActiveMembership = await _memberService.HasActiveMembershipAsync(nationalId);
                if (!hasActiveMembership)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<FinalizeReservationResponse>.Failure("عضویت فعال برای این کد ملی یافت نشد");
                }

                // Validate member eligibility
                var requiredCapabilities = tour.MemberCapabilities.Select(mc => mc.CapabilityId).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                var requiredFeatures = tour.MemberFeatures.Select(mf => mf.FeatureId).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                var requiredAgencies = tour.TourAgencies?.Select(ta => ta.AgencyId).ToList();

                var eligibilityResult = await _memberService.ValidateMemberEligibilityAsync(
                    nationalId,
                    requiredCapabilities.Any() ? requiredCapabilities : null,
                    requiredFeatures.Any() ? requiredFeatures : null,
                    requiredAgencies?.Any() == true ? requiredAgencies : null);

                if (!eligibilityResult.IsEligible)
                {
                    var errorMessage = eligibilityResult.Errors.Any()
                        ? string.Join("، ", eligibilityResult.Errors)
                        : "عضو شرایط لازم برای شرکت در این تور را ندارد";
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<FinalizeReservationResponse>.Failure(errorMessage);
                }

                // Validate participants exist
                if (!reservation.Participants.Any())
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<FinalizeReservationResponse>.Failure("رزرو باید حداقل یک شرکت‌کننده داشته باشد");
                }

                // RE-VALIDATE ALL PARTICIPANTS - Critical step to prevent fraud
                var participantValidationResults = new List<(Participant Participant, ParticipantValidationResult Result)>();
                foreach (var participant in reservation.Participants)
                {
                    var validationRequest = new ParticipantValidationRequest
                    {
                        FirstName = participant.FirstName,
                        LastName = participant.LastName,
                        NationalNumber = participant.NationalNumber,
                        PhoneNumber = participant.PhoneNumber,
                        Email = participant.Email,
                        ParticipantType = participant.ParticipantType.ToString(),
                        BirthDate = participant.BirthDate,
                        EmergencyContactName = participant.EmergencyContactName,
                        EmergencyContactPhone = participant.EmergencyContactPhone,
                        Notes = participant.Notes
                    };

                    var validationResult = await _participantValidationService.ValidateParticipantAsync(
                        validationRequest,
                        cancellationToken);

                    participantValidationResults.Add((participant, validationResult));

                    if (!validationResult.IsValid)
                    {
                        _logger.LogWarning(
                            "Participant validation failed - ReservationId: {ReservationId}, Participant: {NationalNumber}, Type: {Type}, Error: {Error}",
                            request.ReservationId, participant.NationalNumber, participant.ParticipantType, validationResult.ErrorMessage);

                        await _unitOfWork.RollbackAsync(cancellationToken);
                        return ApplicationResult<FinalizeReservationResponse>.Failure(
                            $"اعتبارسنجی شرکت‌کننده با کد ملی {participant.NationalNumber} ناموفق بود: {validationResult.ErrorMessage}");
                    }

                    // Additional validation: If participant type is Member, verify they are actually a member
                    if (participant.ParticipantType == ParticipantType.Member)
                    {
                        var participantNationalId = new NationalId(participant.NationalNumber);
                        var participantMember = await _memberService.GetMemberDetailByNationalCodeAsync(participantNationalId);
                        if (participantMember == null)
                        {
                            _logger.LogWarning(
                                "Participant claimed to be member but is not - ReservationId: {ReservationId}, NationalNumber: {NationalNumber}",
                                request.ReservationId, participant.NationalNumber);

                            await _unitOfWork.RollbackAsync(cancellationToken);
                            return ApplicationResult<FinalizeReservationResponse>.Failure(
                                $"شرکت‌کننده با کد ملی {participant.NationalNumber} ادعای عضویت کرده اما عضو نیست");
                        }

                        // Verify member has active membership
                        var participantHasActiveMembership = await _memberService.HasActiveMembershipAsync(participantNationalId);
                        if (!participantHasActiveMembership)
                        {
                            _logger.LogWarning(
                                "Participant member does not have active membership - ReservationId: {ReservationId}, NationalNumber: {NationalNumber}",
                                request.ReservationId, participant.NationalNumber);

                            await _unitOfWork.RollbackAsync(cancellationToken);
                            return ApplicationResult<FinalizeReservationResponse>.Failure(
                                $"عضویت شرکت‌کننده با کد ملی {participant.NationalNumber} فعال نیست");
                        }
                    }
                    else if (participant.ParticipantType == ParticipantType.Guest)
                    {
                        // For guests, verify they are NOT members (to prevent claiming guest status to bypass restrictions)
                        var participantNationalId = new NationalId(participant.NationalNumber);
                        var participantMember = await _memberService.GetMemberDetailByNationalCodeAsync(participantNationalId);
                        if (participantMember != null && participantMember.HasActiveMembership)
                        {
                            _logger.LogWarning(
                                "Participant claimed to be guest but is actually a member - ReservationId: {ReservationId}, NationalNumber: {NationalNumber}",
                                request.ReservationId, participant.NationalNumber);

                            await _unitOfWork.RollbackAsync(cancellationToken);
                            return ApplicationResult<FinalizeReservationResponse>.Failure(
                                $"شرکت‌کننده با کد ملی {participant.NationalNumber} عضو است و نمی‌تواند به عنوان مهمان ثبت شود");
                        }
                    }
                }

                // Calculate total participants
                var totalParticipants = reservation.Participants.Count;
                var memberParticipants = reservation.Participants.Count(p => p.ParticipantType == ParticipantType.Member);
                var guestParticipants = reservation.Participants.Count(p => p.ParticipantType == ParticipantType.Guest);

                // Validate guest count limits
                if (!tour.CanCreateReservationWithGuests(guestParticipants))
                {
                    var maxGuests = tour.MaxGuestsPerReservation ?? 0;
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<FinalizeReservationResponse>.Failure(
                        $"حداکثر {maxGuests} مهمان در هر رزرو مجاز است");
                }

                // Check capacity
                if (reservation.CapacityId.HasValue)
                {
                    var selectedCapacity = tour.Capacities.FirstOrDefault(c => c.Id == reservation.CapacityId.Value);
                    if (selectedCapacity == null)
                    {
                        await _unitOfWork.RollbackAsync(cancellationToken);
                        return ApplicationResult<FinalizeReservationResponse>.Failure("ظرفیت انتخاب شده یافت نشد");
                    }

                    if (!selectedCapacity.IsActive)
                    {
                        await _unitOfWork.RollbackAsync(cancellationToken);
                        return ApplicationResult<FinalizeReservationResponse>.Failure("ظرفیت انتخاب شده غیرفعال است");
                    }

                    if (!selectedCapacity.IsRegistrationOpen(currentDate))
                    {
                        await _unitOfWork.RollbackAsync(cancellationToken);
                        return ApplicationResult<FinalizeReservationResponse>.Failure("زمان ثبت نام برای این ظرفیت به پایان رسیده است");
                    }

                    if (selectedCapacity.MaxParticipants < totalParticipants)
                    {
                        await _unitOfWork.RollbackAsync(cancellationToken);
                        return ApplicationResult<FinalizeReservationResponse>.Failure(
                            $"تعداد شرکت‌کنندگان ({totalParticipants}) از ظرفیت انتخاب شده ({selectedCapacity.MaxParticipants}) بیشتر است");
                    }

                    // Check capacity utilization
                    var existingReservations = await _reservationRepository.GetByTourIdAndNationalNumberAsync(
                        reservation.TourId, memberDetail.NationalCode, cancellationToken);

                    var currentUtilizationInCapacity = await _reservationRepository.GetCapacityUtilizationAsync(
                        selectedCapacity.Id, cancellationToken);

                    var existingReservationUtilization = existingReservations
                        .Where(r => r.Id != reservation.Id && r.CapacityId == reservation.CapacityId)
                        .Sum(r => r.Participants.Count);

                    var availableSpotsInCapacity = selectedCapacity.MaxParticipants - currentUtilizationInCapacity + existingReservationUtilization;

                    if (availableSpotsInCapacity < totalParticipants)
                    {
                        await _unitOfWork.RollbackAsync(cancellationToken);
                        return ApplicationResult<FinalizeReservationResponse>.Failure(
                            $"ظرفیت انتخاب شده کافی نیست. ظرفیت باقی‌مانده در این ظرفیت: {availableSpotsInCapacity} نفر");
                    }
                }
                else
                {
                    // Check overall tour capacity
                    var existingReservations = await _reservationRepository.GetByTourIdAndNationalNumberAsync(
                        reservation.TourId, memberDetail.NationalCode, cancellationToken);

                    var tourUtilization = await _reservationRepository.GetTourUtilizationAsync(tour.Id, cancellationToken);
                    var existingReservationUtilization = existingReservations
                        .Where(r => r.Id != reservation.Id)
                        .Sum(r => r.Participants.Count);
                    var availableSpots = tour.MaxParticipants - tourUtilization + existingReservationUtilization;

                    if (availableSpots < totalParticipants)
                    {
                        await _unitOfWork.RollbackAsync(cancellationToken);
                        return ApplicationResult<FinalizeReservationResponse>.Failure(
                            $"ظرفیت تور کافی نیست. ظرفیت باقی‌مانده: {availableSpots} نفر");
                    }
                }

                // Check for conflicting reservations
                var allExistingReservations = await _reservationRepository.GetByTourIdAndNationalNumberAsync(
                    reservation.TourId, memberDetail.NationalCode, cancellationToken);

                if (allExistingReservations.Any(r => r.Id != reservation.Id && r.HasConflictingReservation()))
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<FinalizeReservationResponse>.Failure("شما قبلاً برای این تور رزرو فعال دارید");
                }

                // CREATE PRICING SNAPSHOTS using ReservationPricingService
                // This service handles: Guest → Guest pricing, Member → Capability/Feature-based or Default pricing
                var totalAmountRials = 0m;
                var billItems = new List<Nezam.Refahi.Finance.Contracts.Services.BillItemRequest>();

                // Group participants by type for efficient pricing
                var participantsByType = reservation.Participants.GroupBy(p => p.ParticipantType).ToList();

                foreach (var participantGroup in participantsByType)
                {
                    var participantType = participantGroup.Key;
                    var participants = participantGroup.ToList();

                    // Get pricing for first participant in group (all same type should have same pricing)
                    var firstParticipant = participants.First();
                    
                    ReservationPricingResult pricingResult;
                    try
                    {
                        pricingResult = await _reservationPricingService.GetPricingAsync(
                            tourId: tour.Id,
                            nationalNumber: firstParticipant.NationalNumber,
                            memberCapabilities: memberDetail?.Capabilities,
                            memberFeatures: memberDetail?.Features,
                            cancellationToken: cancellationToken);
                    }
                    catch (InvalidOperationException ex)
                    {
                        _logger.LogError(ex, "Failed to get pricing for participant {NationalNumber}", firstParticipant.NationalNumber);
                        await _unitOfWork.RollbackAsync(cancellationToken);
                        return ApplicationResult<FinalizeReservationResponse>.Failure(
                            $"خطا در تعیین قیمت برای شرکت‌کننده با کد ملی {firstParticipant.NationalNumber}: {ex.Message}");
                    }

                    var effectivePriceRials = pricingResult.EffectivePriceRials;
                    var pricingDto = pricingResult.Pricing;
                    var determinedParticipantTypeStr = pricingResult.ParticipantType;
                    
                    // Parse participant type string to enum
                    if (!Enum.TryParse<ParticipantType>(determinedParticipantTypeStr, out var determinedParticipantType))
                    {
                        _logger.LogError("Invalid participant type returned from pricing service: {ParticipantType}", determinedParticipantTypeStr);
                        await _unitOfWork.RollbackAsync(cancellationToken);
                        return ApplicationResult<FinalizeReservationResponse>.Failure(
                            "خطا در تعیین نوع شرکت‌کننده از سرویس قیمت‌گذاری");
                    }

                    // Need to load TourPricing entity for snapshot creation
                    var tourPricingEntity = tour.GetPricing(
                        determinedParticipantType,
                        date: DateTime.UtcNow,
                        memberCapabilities: determinedParticipantType == ParticipantType.Member ? memberDetail?.Capabilities : null,
                        memberFeatures: determinedParticipantType == ParticipantType.Member ? memberDetail?.Features : null);

                    if (tourPricingEntity == null)
                    {
                        _logger.LogError("TourPricing entity not found for PricingId {PricingId}", pricingDto.PricingId);
                        await _unitOfWork.RollbackAsync(cancellationToken);
                        return ApplicationResult<FinalizeReservationResponse>.Failure(
                            "قیمت‌گذاری انتخاب شده یافت نشد");
                    }

                    var effectivePrice = Money.FromRials(effectivePriceRials);

                    // Update participant type if it was incorrectly determined (should match pricing result)
                    foreach (var participant in participants)
                    {
                        // Update required amount for all participants in group
                        participant.UpdateRequiredAmount(effectivePrice);
                        
                        // If participant type doesn't match determined type, log warning (but don't fail)
                        if (participant.ParticipantType != determinedParticipantType)
                        {
                            _logger.LogWarning(
                                "Participant type mismatch for {NationalNumber}. Expected: {ExpectedType}, Found: {ActualType}",
                                participant.NationalNumber, determinedParticipantType, participant.ParticipantType);
                        }
                    }

                    // Calculate total for this group
                    totalAmountRials += effectivePriceRials * participants.Count;

                    // Create price snapshot for this participant type using TourPricing entity
                    var snapshot = ReservationPriceSnapshot.CreateFromPricing(
                        reservationId: reservation.Id,
                        participantType: determinedParticipantType,
                        pricing: tourPricingEntity,
                        memberCapabilityIds: determinedParticipantType == ParticipantType.Member ? memberDetail?.Capabilities : null,
                        memberFeatureIds: determinedParticipantType == ParticipantType.Member ? memberDetail?.Features : null,
                        discountAmount: null,
                        discountCode: null,
                        discountDescription: null,
                        tenantId: _settings.DefaultTenantId);

                    reservation.AddPriceSnapshot(snapshot);

                    // Add to bill items
                    var participantTypeText = determinedParticipantType == ParticipantType.Member ? "عضو" : "مهمان";
                    var participantNames = string.Join("، ", participants.Take(3).Select(p => $"{p.FirstName} {p.LastName}"));
                    if (participants.Count > 3)
                        participantNames += $" و {participants.Count - 3} نفر دیگر";

                    billItems.Add(new Nezam.Refahi.Finance.Contracts.Services.BillItemRequest
                    {
                        Title = $"بلیط تور {tour.Title} - {participantTypeText}",
                        Description = participants.Count == 1
                            ? $"بلیط تور {tour.Title} برای {participantTypeText} ({firstParticipant.FirstName} {firstParticipant.LastName})"
                            : $"بلیط تور {tour.Title} برای {participants.Count} نفر {participantTypeText} ({participantNames})",
                        UnitPriceRials = effectivePriceRials,
                        Quantity = participants.Count,
                        DiscountPercentage = pricingDto.DiscountPercentage
                    });

                    _logger.LogInformation(
                        "Applied pricing for {Count} {Type} participants. PricingId: {PricingId}, IsDefault: {IsDefault}, EffectivePrice: {EffectivePrice}",
                        participants.Count, participantTypeText, pricingDto.PricingId, pricingResult.IsDefaultPricing, effectivePriceRials);
                }

                // Calculate total from snapshots to ensure consistency
                var calculatedTotal = reservation.CalculateTotalFromSnapshots();
                totalAmountRials = calculatedTotal.AmountRials;

                // CREATE BILL using BillService
                var billRequest = new Nezam.Refahi.Finance.Contracts.Services.CreateBillRequest
                {
                    Title = $"فاکتور رزرو تور {tour.Title}",
                    ReferenceTrackingCode = reservation.TrackingCode,
                    ReferenceId = reservation.Id.ToString(),
                    BillType = "TourReservation",
                    UserFullName = memberDetail?.FullName ?? string.Empty,
                    Description = $"فاکتور رزرو تور {tour.Title} با کد پیگیری {reservation.TrackingCode}",
                    DueDate = DateTime.UtcNow.AddMinutes(_settings.ReservationHoldMinutes), // Bill due date matches reservation expiry
                    Metadata = new Dictionary<string, string>
                    {
                        ["ReservationId"] = reservation.Id.ToString(),
                        ["ReservationTrackingCode"] = reservation.TrackingCode,
                        ["TourId"] = tour.Id.ToString(),
                        ["TourTitle"] = tour.Title,
                        ["ParticipantCount"] = totalParticipants.ToString(),
                        ["MemberParticipantCount"] = memberParticipants.ToString(),
                        ["GuestParticipantCount"] = guestParticipants.ToString()
                    },
                    Items = billItems
                };

                var billResult = await _billService.CreateAndIssueBillAsync(billRequest, cancellationToken);
                if (!billResult.IsSuccess)
                {
                    _logger.LogError("Failed to create bill for reservation {ReservationId}: {Error}",
                        request.ReservationId, billResult.Message);
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<FinalizeReservationResponse>.Failure(
                        $"خطا در ایجاد فاکتور: {billResult.Message}");
                }

                // Set bill ID on reservation
                reservation.SetBillId(billResult.Data!.BillId);

                // Set expiry date
                var expiryDate = DateTime.UtcNow.AddMinutes(_settings.ReservationHoldMinutes);
                reservation.UpdateExpiryDate(expiryDate);

                // HOLD RESERVATION (transition to OnHold status)
                reservation.Hold();

                // Save reservation changes
                await _reservationRepository.UpdateAsync(reservation, cancellationToken: cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Commit transaction
                await _unitOfWork.CommitAsync(cancellationToken);

                _logger.LogInformation(
                    "Successfully finalized reservation - ReservationId: {ReservationId}, Status: {Status}, BillId: {BillId}, TotalAmount: {TotalAmount}",
                    reservation.Id, reservation.Status, billResult.Data.BillId, totalAmountRials);

                // Publish ReservationHeldIntegrationEvent
                var reservationHeldEvent = new ReservationHeldIntegrationEvent
                {
                    ReservationId = reservation.Id,
                    TrackingCode = reservation.TrackingCode,
                    TourId = tour.Id,
                    TourTitle = tour.Title,
                    TourStartDate = tour.TourStart,
                    TourEndDate = tour.TourEnd,
                    ReservationDate = reservation.ReservationDate,
                    ExpiryDate = reservation.ExpiryDate,
                    HeldAt = DateTime.UtcNow,
                    ExternalUserId = userId.Value,
                    UserFullName = memberDetail?.FullName ?? string.Empty,
                    UserNationalCode = memberDetail?.NationalCode ?? string.Empty,
                    MemberId = reservation.MemberId,
                    Status = reservation.Status.ToString(),
                    PreviousStatus = ReservationStatus.Draft.ToString(),
                    TotalAmountRials = totalAmountRials,
                    Currency = "IRR",
                    CapacityId = reservation.CapacityId,
                    CapacityName = reservation.CapacityId.HasValue
                        ? tour.Capacities.FirstOrDefault(c => c.Id == reservation.CapacityId.Value)?.Description
                        : null,
                    ParticipantCount = reservation.Participants.Count,
                    MemberParticipantCount = reservation.Participants.Count(p => p.ParticipantType == ParticipantType.Member),
                    GuestParticipantCount = reservation.Participants.Count(p => p.ParticipantType == ParticipantType.Guest),
                    Participants = reservation.Participants.Select(p => new ReservationParticipantDto
                    {
                        Name = $"{p.FirstName} {p.LastName}".Trim(),
                        NationalCode = p.NationalNumber,
                        PhoneNumber = p.PhoneNumber,
                        Email = p.Email,
                        IsMainParticipant = p.ParticipantType == ParticipantType.Member,
                        Age = _validationService.CalculateAge(p.BirthDate, tour.TourStart)
                    }).ToList(),
                    PriceSnapshots = reservation.PriceSnapshots.Select(ps => new ReservationPriceSnapshotDto
                    {
                        SnapshotId = ps.Id,
                        ParticipantType = ps.ParticipantType.ToString(),
                        BasePriceRials = ps.BasePrice.AmountRials,
                        FinalPriceRials = ps.FinalPrice.AmountRials,
                        DiscountAmountRials = ps.DiscountAmount?.AmountRials,
                        DiscountCode = ps.DiscountCode,
                        DiscountDescription = ps.DiscountDescription,
                        SnapshotDate = ps.SnapshotDate,
                        TourPricingId = ps.TourPricingId
                    }).ToList(),
                    Metadata = new Dictionary<string, string>
                    {
                        ["TourId"] = tour.Id.ToString(),
                        ["TourTitle"] = tour.Title,
                        ["ReservationId"] = reservation.Id.ToString(),
                        ["TrackingCode"] = reservation.TrackingCode,
                        ["BillId"] = billResult.Data.BillId.ToString(),
                        ["BillNumber"] = billResult.Data.BillNumber,
                        ["ReservationDate"] = reservation.ReservationDate.ToString("O"),
                        ["ExpiryDate"] = reservation.ExpiryDate?.ToString("O") ?? string.Empty,
                        ["HeldAt"] = DateTime.UtcNow.ToString("O"),
                        ["PreviousStatus"] = ReservationStatus.Draft.ToString(),
                        ["CurrentStatus"] = reservation.Status.ToString(),
                        ["Action"] = "Finalize"
                    }
                };

                await _publishEndpoint.Publish(reservationHeldEvent, cancellationToken);

                var response = new FinalizeReservationResponse
                {
                    ReservationId = reservation.Id,
                    TrackingCode = reservation.TrackingCode,
                    Status = reservation.Status.ToString(),
                    BillId = billResult.Data.BillId,
                    BillNumber = billResult.Data.BillNumber,
                    TotalAmountRials = totalAmountRials,
                    ExpiryDate = expiryDate,
                    ParticipantCount = reservation.Participants.Count,
                    TourTitle = tour.Title
                };

                return ApplicationResult<FinalizeReservationResponse>.Success(response, "رزرو با موفقیت نهایی و نگه‌داری شد");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Invalid operation while finalizing reservation - ReservationId: {ReservationId}",
                    request.ReservationId);
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<FinalizeReservationResponse>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while finalizing reservation - ReservationId: {ReservationId}",
                    request.ReservationId);
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<FinalizeReservationResponse>.Failure(ex, "خطا در نهایی‌سازی رزرو");
            }
        }
        finally
        {
            // Release lock
            semaphore.Release();

            // Clean up lock if reservation is finalized (no longer needed)
            // Note: We keep the lock entry to avoid recreation overhead, but semaphore is released
        }
    }
}

