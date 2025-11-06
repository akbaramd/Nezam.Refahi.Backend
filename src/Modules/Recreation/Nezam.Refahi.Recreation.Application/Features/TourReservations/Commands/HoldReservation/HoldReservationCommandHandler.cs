using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nezam.Refahi.Membership.Contracts.Dtos;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Recreation.Application.Configuration;
using Nezam.Refahi.Recreation.Application.Services.Contracts;
using Nezam.Refahi.Recreation.Contracts.Dtos;
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
using Nezam.Refahi.Recreation.Application.Services;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.HoldReservation;

/// <summary>
/// Handler for holding a draft reservation
/// Performs all validation checks including capacity, member capabilities/features, pricing, and creates snapshots
/// </summary>
public class HoldReservationCommandHandler : IRequestHandler<HoldReservationCommand, ApplicationResult<HoldReservationResponse>>
{
    private readonly ITourReservationRepository _reservationRepository;
    private readonly ITourRepository _tourRepository;
    private readonly IRecreationUnitOfWork _unitOfWork;
    private readonly IMemberService _memberService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IParticipantValidationService _participantValidationService;
    private readonly IValidationService _validationService;
    private readonly ReservationSettings _settings;
    private readonly ILogger<HoldReservationCommandHandler> _logger;
    private readonly IBus _publishEndpoint;

    public HoldReservationCommandHandler(
        ITourReservationRepository reservationRepository,
        ITourRepository tourRepository,
        IRecreationUnitOfWork unitOfWork,
        IMemberService memberService,
        ICurrentUserService currentUserService,
        IParticipantValidationService participantValidationService,
        IValidationService validationService,
        IOptions<ReservationSettings> settings,
        ILogger<HoldReservationCommandHandler> logger,
        IBus publishEndpoint)
    {
        _reservationRepository = reservationRepository;
        _tourRepository = tourRepository;
        _unitOfWork = unitOfWork;
        _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
        _currentUserService = currentUserService;
        _participantValidationService = participantValidationService;
        _validationService = validationService;
        _settings = settings.Value;
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<ApplicationResult<HoldReservationResponse>> Handle(
        HoldReservationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Holding reservation - ReservationId: {ReservationId}", request.ReservationId);

            // Get reservation with all related data
            var reservation = await _reservationRepository.FindOneAsync(
                x => x.Id == request.ReservationId,
                cancellationToken);

            if (reservation == null)
            {
                _logger.LogWarning("Reservation not found - ReservationId: {ReservationId}", request.ReservationId);
                return ApplicationResult<HoldReservationResponse>.Failure("رزرو مورد نظر یافت نشد");
            }

            // Validate reservation can be held (domain behavior)
            if (!reservation.CanHold(out var canHoldError))
            {
                _logger.LogWarning("Reservation cannot be held - ReservationId: {ReservationId}, Error: {Error}",
                    request.ReservationId, canHoldError);
                return ApplicationResult<HoldReservationResponse>.Failure(canHoldError!);
            }

            // Get tour with capacities and pricing
            var tour = await _tourRepository.FindOneAsync(x => x.Id == reservation.TourId, cancellationToken);
            if (tour == null)
            {
                return ApplicationResult<HoldReservationResponse>.Failure("تور مورد نظر یافت نشد");
            }

            // Validate tour is active and registration is open
            if (!tour.IsActive)
            {
                return ApplicationResult<HoldReservationResponse>.Failure("تور غیرفعال است");
            }

            var currentDate = DateTime.UtcNow;
            if (!tour.IsRegistrationOpen(currentDate))
            {
                return ApplicationResult<HoldReservationResponse>.Failure("زمان ثبت نام برای این تور به پایان رسیده است");
            }

            // Check if tour is too close to start date
            var hoursUntilTourStart = (tour.TourStart - DateTime.UtcNow).TotalHours;
            if (hoursUntilTourStart < _settings.MinimumHoursBeforeTour)
            {
                return ApplicationResult<HoldReservationResponse>.Failure(
                    $"امکان رزرو کمتر از {_settings.MinimumHoursBeforeTour} ساعت قبل از شروع تور وجود ندارد");
            }

            // Get member information
            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
            {
                return ApplicationResult<HoldReservationResponse>.Failure("کاربر درخواست کننده یافت نشد");
            }

            // Get member by external user ID
            var memberDto = await _memberService.GetMemberByExternalIdAsync(userId.Value.ToString());
            if (memberDto == null)
            {
                return ApplicationResult<HoldReservationResponse>.Failure("عضو یافت نشد");
            }

            // Get detailed member information including capabilities and features
            var nationalId = new NationalId(memberDto.NationalCode);
            var memberDetail = await _memberService.GetMemberDetailByNationalCodeAsync(nationalId);
            if (memberDetail == null)
            {
                return ApplicationResult<HoldReservationResponse>.Failure("اطلاعات عضو یافت نشد");
            }

            // Validate member has active membership
            var hasActiveMembership = await _memberService.HasActiveMembershipAsync(nationalId);
            if (!hasActiveMembership)
            {
                return ApplicationResult<HoldReservationResponse>.Failure("عضویت فعال برای این کد ملی یافت نشد");
            }

            // Validate member eligibility using the new service method
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
                return ApplicationResult<HoldReservationResponse>.Failure(errorMessage);
            }

            // Validate main participant's national ID format
            if (!_validationService.IsValidNationalId(memberDetail.NationalCode))
            {
                return ApplicationResult<HoldReservationResponse>.Failure("کد ملی شرکت‌کننده اصلی معتبر نیست");
            }

            // Check restricted tours
            var restrictedTourValidation = await ValidateRestrictedToursAsync(tour, nationalId, cancellationToken);
            if (!restrictedTourValidation.IsSuccess)
            {
                return ApplicationResult<HoldReservationResponse>.Failure(restrictedTourValidation.Errors);
            }

            // Check for conflicting reservations (only Paying and Confirmed prevent new reservations)
            var existingReservations = await _reservationRepository.GetByTourIdAndNationalNumberAsync(
                reservation.TourId, memberDetail.NationalCode, cancellationToken);

            if (existingReservations.Any(r => r.Id != reservation.Id && r.HasConflictingReservation()))
            {
                return ApplicationResult<HoldReservationResponse>.Failure("شما قبلاً برای این تور رزرو فعال دارید");
            }

            // Validate participants exist
            if (!reservation.Participants.Any())
            {
                return ApplicationResult<HoldReservationResponse>.Failure("رزرو باید حداقل یک شرکت‌کننده داشته باشد");
            }

            // Calculate total participants (member + guests)
            var totalParticipants = reservation.Participants.Count;
            var memberParticipants = reservation.Participants.Count(p => p.ParticipantType == ParticipantType.Member);
            var guestParticipants = reservation.Participants.Count(p => p.ParticipantType == ParticipantType.Guest);

            // Validate guest count limits
            if (!tour.CanCreateReservationWithGuests(guestParticipants))
            {
                var maxGuests = tour.MaxGuestsPerReservation ?? 0;
                return ApplicationResult<HoldReservationResponse>.Failure(
                    $"حداکثر {maxGuests} مهمان در هر رزرو مجاز است");
            }

            // Check capacity if capacity is specified
            if (reservation.CapacityId.HasValue)
            {
                var selectedCapacity = tour.Capacities.FirstOrDefault(c => c.Id == reservation.CapacityId.Value);
                if (selectedCapacity == null)
                {
                    return ApplicationResult<HoldReservationResponse>.Failure("ظرفیت انتخاب شده یافت نشد");
                }

                if (!selectedCapacity.IsActive)
                {
                    return ApplicationResult<HoldReservationResponse>.Failure("ظرفیت انتخاب شده غیرفعال است");
                }

                if (!selectedCapacity.IsRegistrationOpen(currentDate))
                {
                    return ApplicationResult<HoldReservationResponse>.Failure("زمان ثبت نام برای این ظرفیت به پایان رسیده است");
                }

                // Validate that participants fit in selected capacity
                if (selectedCapacity.MaxParticipants < totalParticipants)
                {
                    return ApplicationResult<HoldReservationResponse>.Failure(
                        $"تعداد شرکت‌کنندگان ({totalParticipants}) از ظرفیت انتخاب شده ({selectedCapacity.MaxParticipants}) بیشتر است");
                }

                // Check if selected capacity has enough available spots (including this reservation)
                var currentUtilizationInCapacity = await _reservationRepository.GetCapacityUtilizationAsync(
                    selectedCapacity.Id, cancellationToken);

                // Subtract current reservation participants if it's already counted
                var existingReservationUtilization = existingReservations
                    .Where(r => r.Id == reservation.Id && r.CapacityId == reservation.CapacityId)
                    .Sum(r => r.Participants.Count);

                var availableSpotsInCapacity = selectedCapacity.MaxParticipants - currentUtilizationInCapacity + existingReservationUtilization;

                if (availableSpotsInCapacity < totalParticipants)
                {
                    return ApplicationResult<HoldReservationResponse>.Failure(
                        $"ظرفیت انتخاب شده کافی نیست. ظرفیت باقی‌مانده در این ظرفیت: {availableSpotsInCapacity} نفر");
                }
            }
            else
            {
                // Check overall tour capacity
                var tourUtilization = await _reservationRepository.GetTourUtilizationAsync(tour.Id, cancellationToken);
                var existingReservationUtilization = existingReservations
                    .Where(r => r.Id == reservation.Id)
                    .Sum(r => r.Participants.Count);
                var availableSpots = tour.MaxParticipants - tourUtilization + existingReservationUtilization;

                if (availableSpots < totalParticipants)
                {
                    return ApplicationResult<HoldReservationResponse>.Failure(
                        $"ظرفیت تور کافی نیست. ظرفیت باقی‌مانده: {availableSpots} نفر");
                }
            }

            // Get active pricing for the tour
            var activePricing = tour.GetActivePricing();
            if (!activePricing.Any())
            {
                return ApplicationResult<HoldReservationResponse>.Failure("قیمت‌گذاری برای این تور یافت نشد");
            }

            // Calculate pricing and create snapshots for all participants
            var totalAmountRials = 0m;

            // Process main participant (member)
            var mainParticipant = reservation.Participants.FirstOrDefault(p => p.ParticipantType == ParticipantType.Member);
            if (mainParticipant != null)
            {
                var memberPricing = activePricing.FirstOrDefault(p => p.ParticipantType == ParticipantType.Member);
                if (memberPricing == null)
                {
                    return ApplicationResult<HoldReservationResponse>.Failure("قیمت‌گذاری برای شرکت‌کننده اصلی یافت نشد");
                }

                var memberPrice = memberPricing.GetEffectivePrice();
                totalAmountRials += memberPrice.AmountRials;

                // Update participant required amount
                mainParticipant.UpdateRequiredAmount(memberPrice);

                // Create or update price snapshot for main participant
                var mainSnapshot = new ReservationPriceSnapshot(
                    reservationId: reservation.Id,
                    participantType: ParticipantType.Member,
                    basePrice: memberPrice,
                    finalPrice: memberPrice,
                    discountAmount: null,
                    discountCode: null,
                    discountDescription: null,
                    pricingRules: JsonSerializer.Serialize(new
                    {
                        PricingId = memberPricing.Id,
                        ParticipantType = ParticipantType.Member.ToString(),
                        BasePrice = memberPrice.AmountRials,
                        AppliedAt = DateTime.UtcNow
                    }),
                    tenantId: _settings.DefaultTenantId);

                reservation.AddPriceSnapshot(mainSnapshot);
            }

            // Process guest participants
            var guestParticipantsList = reservation.Participants.Where(p => p.ParticipantType == ParticipantType.Guest).ToList();
            if (guestParticipantsList.Any())
            {
                var guestPricing = activePricing.FirstOrDefault(p => p.ParticipantType == ParticipantType.Guest);
                if (guestPricing == null)
                {
                    return ApplicationResult<HoldReservationResponse>.Failure("قیمت‌گذاری برای مهمان یافت نشد");
                }

                var guestPrice = guestPricing.GetEffectivePrice();
                totalAmountRials += guestPrice.AmountRials * guestParticipantsList.Count;

                // Update all guest participants required amount
                foreach (var guestParticipant in guestParticipantsList)
                {
                    guestParticipant.UpdateRequiredAmount(guestPrice);
                }

                // Create or update price snapshot for guest participants
                var guestSnapshot = new ReservationPriceSnapshot(
                    reservationId: reservation.Id,
                    participantType: ParticipantType.Guest,
                    basePrice: guestPrice,
                    finalPrice: guestPrice,
                    discountAmount: null,
                    discountCode: null,
                    discountDescription: null,
                    pricingRules: JsonSerializer.Serialize(new
                    {
                        PricingId = guestPricing.Id,
                        ParticipantType = ParticipantType.Guest.ToString(),
                        BasePrice = guestPrice.AmountRials,
                        AppliedAt = DateTime.UtcNow,
                        GuestCount = guestParticipantsList.Count
                    }),
                    tenantId: _settings.DefaultTenantId);

                reservation.AddPriceSnapshot(guestSnapshot);
            }

            // Set expiry date
            var expiryDate = DateTime.UtcNow.AddMinutes(_settings.ReservationHoldMinutes);
            reservation.UpdateExpiryDate(expiryDate);

            // Calculate total amount from snapshots before holding
            var calculatedTotal = reservation.CalculateTotalFromSnapshots();
            
            // Transition to OnHold status (using Hold method which transitions to Held state)
            // The Hold() method will validate participants exist
            reservation.Hold();

            // Update total amount after hold (TotalAmount is set via Confirm method, but we need to set it here)
            // Since TotalAmount has private setter, we'll use reflection or check if there's a method
            // Actually, looking at Confirm method, it accepts totalAmount parameter
            // But Hold() doesn't. Let's calculate total from snapshots after hold
            // Note: TotalAmount will be calculated from snapshots when needed
            // For now, we'll use the calculated total we got from snapshots
            totalAmountRials = calculatedTotal.AmountRials;

            // Save changes
            await _reservationRepository.UpdateAsync(reservation, cancellationToken: cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully held reservation - ReservationId: {ReservationId}, Status: {Status}, TotalAmount: {TotalAmount}",
                reservation.Id, reservation.Status, totalAmountRials);

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
                UserFullName = memberDetail.FullName ?? string.Empty,
                UserNationalCode = memberDetail.NationalCode,
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
                    ["ReservationDate"] = reservation.ReservationDate.ToString("O"),
                    ["ExpiryDate"] = reservation.ExpiryDate?.ToString("O") ?? string.Empty,
                    ["HeldAt"] = DateTime.UtcNow.ToString("O"),
                    ["PreviousStatus"] = ReservationStatus.Draft.ToString(),
                    ["CurrentStatus"] = reservation.Status.ToString(),
                    ["Action"] = "Hold"
                }
            };

            await _publishEndpoint.Publish(reservationHeldEvent, cancellationToken);

            var response = new HoldReservationResponse
            {
                ReservationId = reservation.Id,
                TrackingCode = reservation.TrackingCode,
                Status = reservation.Status.ToString(),
                TotalAmountRials = totalAmountRials,
                ExpiryDate = expiryDate,
                ParticipantCount = reservation.Participants.Count,
                TourTitle = tour.Title
            };

            return ApplicationResult<HoldReservationResponse>.Success(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while holding reservation - ReservationId: {ReservationId}",
                request.ReservationId);
            return ApplicationResult<HoldReservationResponse>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while holding reservation - ReservationId: {ReservationId}",
                request.ReservationId);
            return ApplicationResult<HoldReservationResponse>.Failure(ex, "خطا در نگهداری رزرو");
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
            restrictedTourIds, nationalId.Value, cancellationToken);

        var activeReservations = existingReservations.Where(r => r.IsActive()).ToList();

        if (activeReservations.Any())
        {
            var restrictedTourTitles = string.Join("، ", activeReservations.Select(r => r.Tour?.Title ?? "Unknown"));
            return ApplicationResult.Failure(
                $"شما در تورهای محدودکننده زیر رزرو دارید و نمی‌توانید برای این تور رزرو کنید: {restrictedTourTitles}");
        }

        return ApplicationResult.Success();
    }
}

