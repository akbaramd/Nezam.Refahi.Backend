using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Membership.Contracts.Dtos;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Recreation.Application.Services;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.StartReservation;

public sealed class StartReservationCommandHandler
    : IRequestHandler<StartReservationCommand, ApplicationResult<StartReservationCommandResult>>
{
    private const int DEFAULT_ALLOCATION = 1;

    private readonly ITourRepository _tourRepository;
    private readonly ITourReservationRepository _reservationRepository;
    private readonly IMemberService _memberService;
    private readonly IRecreationUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<StartReservationCommandHandler> _logger;

    public StartReservationCommandHandler(
        ITourRepository tourRepository,
        ITourReservationRepository reservationRepository,
        IMemberService memberService,
        IRecreationUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<StartReservationCommandHandler> logger)
    {
        _tourRepository = tourRepository ?? throw new ArgumentNullException(nameof(tourRepository));
        _reservationRepository = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
        _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult<StartReservationCommandResult>> Handle(StartReservationCommand request, CancellationToken ct)
    {
        if (request.TourId == Guid.Empty)
            return ApplicationResult<StartReservationCommandResult>.Failure("شناسه تور معتبر نیست");
        if (request.CapacityId == Guid.Empty)
            return ApplicationResult<StartReservationCommandResult>.Failure("ظرفیت انتخاب‌شده الزامی است");
        if (string.IsNullOrWhiteSpace(request.UserNationalNumber))
            return ApplicationResult<StartReservationCommandResult>.Failure("کد ملی کاربر الزامی است");

        // Ensure user is authenticated and has a valid user id (ExternalUserId comes from current user service)
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
            return ApplicationResult<StartReservationCommandResult>.Failure("کاربر احراز هویت نشده است");

        // Load Tour with Capacities tracked
        var tour = await _tourRepository.FindOneAsync(t => t.Id == request.TourId, ct);
        if (tour is null)
            return ApplicationResult<StartReservationCommandResult>.Failure("تور یافت نشد");
        if (!tour.IsActive)
            return ApplicationResult<StartReservationCommandResult>.Failure("تور غیرفعال است");
        if (!tour.IsRegistrationOpen(DateTime.UtcNow))
            return ApplicationResult<StartReservationCommandResult>.Failure("ثبت‌نام برای این تور باز نیست");

        // Member validation using eligibility check
        var nationalId = new NationalId(request.UserNationalNumber);
        
        // Get required capabilities, features, and agencies from tour
        var requiredCapabilities = tour.MemberCapabilities.Select(mc => mc.CapabilityId).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
        var requiredFeatures = tour.MemberFeatures.Select(mf => mf.FeatureId).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
        var requiredAgencies = tour.TourAgencies?.Select(ta => ta.AgencyId).ToList();

        // Validate member eligibility
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
            return ApplicationResult<StartReservationCommandResult>.Failure(errorMessage);
        }

        var memberDetail = eligibilityResult.MemberDetail;
        if (memberDetail == null)
            return ApplicationResult<StartReservationCommandResult>.Failure("اطلاعات عضو یافت نشد");

        // Restriction check
        var restrictedCheck = await ValidateRestrictedToursAsync(tour, nationalId, ct);
        if (!restrictedCheck.IsSuccess)
            return ApplicationResult<StartReservationCommandResult>.Failure(restrictedCheck.Errors);

        // Capacity validation (must belong to tour)
        var capacity = tour.Capacities?.FirstOrDefault(c => c.Id == request.CapacityId);
        if (capacity is null)
            return ApplicationResult<StartReservationCommandResult>.Failure("ظرفیت انتخاب‌شده متعلق به این تور نیست");

        var isVip = IsSpecialMember(memberDetail);
        if (!capacity.IsVisibleToMember(isVip))
            return ApplicationResult<StartReservationCommandResult>.Failure("دسترسی به این ظرفیت برای شما مجاز نیست");
        if (!capacity.CanAccommodateForMember(DEFAULT_ALLOCATION, isVip))
            return ApplicationResult<StartReservationCommandResult>.Failure("ظرفیت انتخاب‌شده شرایط یا ظرفیت کافی را ندارد");

        // Prevent multiple drafts/held for same tour/member
        var existing = await _reservationRepository
            .GetByTourIdAndNationalNumberAsync(request.TourId, nationalId.Value, ct);

        if (existing.Any(r => r.Status == ReservationStatus.Draft ||
                              (r.Status == ReservationStatus.OnHold && !r.IsExpired())))
        {
            return ApplicationResult<StartReservationCommandResult>.Failure("پیش‌نویس/رزرو فعال قبلی برای این تور موجود است");
        }

        try
        {
            await _unitOfWork.BeginAsync(ct);

            // Atomic allocate 1 seat
            if (!capacity.TryAllocateParticipants(DEFAULT_ALLOCATION))
                return ApplicationResult<StartReservationCommandResult>.Failure("ظرفیت انتخاب‌شده هم‌اکنون قابل تخصیص نیست");

            var trackingCode = await GenerateUniqueTrackingCodeAsync(ct);

            var reservation = new TourReservation(
                tourId: tour.Id,
                trackingCode: trackingCode,
                externalUserId: _currentUserService.UserId.Value,
                capacityId: capacity.Id,
                memberId: memberDetail.Id,
                expiryDate: null,
                notes: null);

            await _reservationRepository.AddAsync(reservation, cancellationToken:ct);
            await _unitOfWork.SaveChangesAsync(ct);

            // Add current user as a participant
            await AddCurrentUserAsParticipantAsync(reservation, tour, memberDetail, nationalId, ct);

            await _unitOfWork.SaveChangesAsync(ct);
            await _unitOfWork.CommitAsync(ct);

            return ApplicationResult<StartReservationCommandResult>.Success(new StartReservationCommandResult
            {
                ReservationId = reservation.Id
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed StartReservation TourId={TourId} Nat={Nat}", request.TourId, request.UserNationalNumber);
            try { await _unitOfWork.RollbackAsync(ct); } catch { /* ignore */ }
            return ApplicationResult<StartReservationCommandResult>.Failure("خطا در ایجاد رزرو");
        }
    }

    // Helpers

    private async Task<string> GenerateUniqueTrackingCodeAsync(CancellationToken ct)
    {
        for (int i = 0; i < 7; i++)
        {
            var code = $"RSV-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(100, 999)}";
            var exists = await _reservationRepository.TrackingCodeExistsAsync(code, ct);
            if (!exists) return code;
            await Task.Delay(5, ct);
        }
        throw new ApplicationException("امکان تولید کد پیگیری یکتا فراهم نشد");
    }

    private async Task<ApplicationResult> ValidateRestrictedToursAsync(Tour tour, NationalId nationalId, CancellationToken ct)
    {
        if (!tour.TourRestrictedTours.Any())
            return ApplicationResult.Success();

        var restrictedIds = tour.TourRestrictedTours.Select(r => r.RestrictedTourId).ToList();
        var reservations = await _reservationRepository.GetByTourIdsAndNationalNumberAsync(restrictedIds, nationalId.Value, ct);
        var active = reservations.Where(r => r.IsActive()).ToList();
        if (active.Any())
        {
            var titles = string.Join("، ", active.Select(r => r.Tour.Title));
            return ApplicationResult.Failure($"به‌دلیل رزرو فعال در تورهای محدودکننده، امکان رزرو این تور وجود ندارد: {titles}");
        }
        return ApplicationResult.Success();
    }

    private static bool IsSpecialMember(MemberDetailDto member)
    {
        var caps = member.Capabilities ?? Enumerable.Empty<string>();
        var feats = member.Features ?? Enumerable.Empty<string>();
        return caps.Contains("VIP", StringComparer.OrdinalIgnoreCase)
            || feats.Contains("VIP", StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Adds the current user as a participant to the reservation
    /// </summary>
    private async Task AddCurrentUserAsParticipantAsync(
        TourReservation reservation,
        Tour tour,
        MemberDetailDto memberDetail,
        NationalId nationalId,
        CancellationToken ct)
    {
        try
        {
            // Determine participant type based on active membership
            var hasActiveMembership = await _memberService.HasActiveMembershipAsync(nationalId);
            var participantType = hasActiveMembership ? ParticipantType.Member : ParticipantType.Guest;

            // Get pricing for the determined participant type
            var pricingList = tour.GetActivePricing();
            var participantPricing = pricingList.FirstOrDefault(p => p.ParticipantType == participantType);
            
            if (participantPricing == null)
            {
                var participantTypeText = participantType == ParticipantType.Member ? "عضو" : "مهمان";
                _logger.LogWarning(
                    "Pricing not found for participant type {ParticipantType} in tour {TourId}",
                    participantTypeText, tour.Id);
                throw new InvalidOperationException($"قیمت‌گذاری برای {participantTypeText} یافت نشد");
            }

            var requiredPrice = participantPricing.GetEffectivePrice(); // Money

            // Extract user information from member detail
            var firstName = memberDetail.FirstName ?? string.Empty;
            var lastName = memberDetail.LastName ?? string.Empty;
            var phoneNumber = memberDetail.PhoneNumber ?? string.Empty;
            var email = memberDetail.Email;
            var birthDate = memberDetail.BirthDate ?? DateTime.UtcNow.AddYears(-30); // Default if not available

            // Create Participant for the current user
            var participant = new Participant(
                reservationId: reservation.Id,
                firstName: firstName,
                lastName: lastName,
                nationalNumber: nationalId.Value,
                phoneNumber: phoneNumber,
                birthDate: birthDate,
                participantType: participantType,
                requiredAmount: requiredPrice,
                email: email,
                emergencyContactName: null,
                emergencyContactPhone: null,
                notes: "کاربر اصلی رزرو");

            // Add participant to reservation
            reservation.AddParticipant(participant);

            _logger.LogInformation(
                "Added current user {NationalNumber} as {ParticipantType} participant to reservation {ReservationId}",
                nationalId.Value,
                participantType == ParticipantType.Member ? "Member" : "Guest",
                reservation.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to add current user as participant to reservation {ReservationId}",
                reservation.Id);
            throw;
        }
    }
}
