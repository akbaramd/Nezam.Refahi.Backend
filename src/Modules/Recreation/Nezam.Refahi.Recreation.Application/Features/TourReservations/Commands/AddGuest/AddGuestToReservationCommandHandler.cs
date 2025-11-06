using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Recreation.Application.Services;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Contracts.Services;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.AddGuest;

public sealed class AddGuestToReservationCommandHandler
    : IRequestHandler<AddGuestToReservationCommand, ApplicationResult<AddGuestToReservationResponse>>
{
    private readonly ITourReservationRepository _reservationRepository;
    private readonly ITourRepository _tourRepository;
    private readonly IRecreationUnitOfWork _unitOfWork;
    private readonly IParticipantValidationService _participantValidationService;
    private readonly IMemberService _memberService;
    private readonly ILogger<AddGuestToReservationCommandHandler> _logger;

    public AddGuestToReservationCommandHandler(
        ITourReservationRepository reservationRepository,
        ITourRepository tourRepository,
        IRecreationUnitOfWork unitOfWork,
        IParticipantValidationService participantValidationService,
        IMemberService memberService,
        ILogger<AddGuestToReservationCommandHandler> logger)
    {
        _reservationRepository        = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
        _tourRepository               = tourRepository ?? throw new ArgumentNullException(nameof(tourRepository));
        _unitOfWork                   = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _participantValidationService = participantValidationService ?? throw new ArgumentNullException(nameof(participantValidationService));
        _memberService                = memberService ?? throw new ArgumentNullException(nameof(memberService));
        _logger                       = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult<AddGuestToReservationResponse>> Handle(
        AddGuestToReservationCommand request,
        CancellationToken ct)
    {
        if (request.ReservationId == Guid.Empty)
            return ApplicationResult<AddGuestToReservationResponse>.Failure("شناسه رزرو معتبر نیست");
        if (request.ExternalUserId == Guid.Empty)
            return ApplicationResult<AddGuestToReservationResponse>.Failure("شناسه کاربر الزامی است");
        if (string.IsNullOrWhiteSpace(request.Guest?.NationalNumber))
            return ApplicationResult<AddGuestToReservationResponse>.Failure("کد ملی مهمان الزامی است");

        try
        {
            // Determine participant type automatically based on national number
            // Check if person with this national number is a member
            var nationalId = new NationalId(request.Guest.NationalNumber);
            var memberDetail = await _memberService.GetMemberDetailByNationalCodeAsync(nationalId);
            
            ParticipantType determinedParticipantType;
            bool isMember = false;

            if (memberDetail != null)
            {
                // Check if member has active membership
                var hasActiveMembership = await _memberService.HasActiveMembershipAsync(nationalId);
                if (hasActiveMembership)
                {
                    determinedParticipantType = ParticipantType.Member;
                    isMember = true;
                    
                    _logger.LogInformation(
                        "Participant with national number {NationalNumber} is identified as Member (MembershipNumber: {MembershipNumber})",
                        request.Guest.NationalNumber, memberDetail.MembershipNumber);
                }
                else
                {
                    // Member exists but membership is not active - treat as guest
                    determinedParticipantType = ParticipantType.Guest;
                    _logger.LogInformation(
                        "Participant with national number {NationalNumber} is a member but membership is inactive - treating as Guest",
                        request.Guest.NationalNumber);
                }
            }
            else
            {
                // Not a member - treat as guest
                determinedParticipantType = ParticipantType.Guest;
                _logger.LogInformation(
                    "Participant with national number {NationalNumber} is not a member - treating as Guest",
                    request.Guest.NationalNumber);
            }

            // Convert GuestParticipantDto to ParticipantValidationRequest with auto-determined type
            var validationRequest = new ParticipantValidationRequest
            {
                FirstName = request.Guest.FirstName,
                LastName = request.Guest.LastName,
                NationalNumber = request.Guest.NationalNumber,
                PhoneNumber = request.Guest.PhoneNumber,
                Email = request.Guest.Email,
                ParticipantType = determinedParticipantType.ToString(), // Use auto-determined type
                BirthDate = request.Guest.BirthDate,
                EmergencyContactName = request.Guest.EmergencyContactName,
                EmergencyContactPhone = request.Guest.EmergencyContactPhone,
                Notes = request.Guest.Notes
            };

            // Validate participant basic/profile constraints
            var validation = await _participantValidationService.ValidateParticipantAsync(validationRequest, ct);
            if (!validation.IsValid)
                return ApplicationResult<AddGuestToReservationResponse>.Failure(validation.ErrorMessage!);

            // Load reservation (with Participants)
            var reservation = await _reservationRepository.GetByIdAsync(request.ReservationId, ct);
            if (reservation is null)
                return ApplicationResult<AddGuestToReservationResponse>.Failure("رزرو یافت نشد");

            // Validate reservation can accept participants (domain behavior)
            if (!reservation.CanAddParticipant(out var canAddError))
                return ApplicationResult<AddGuestToReservationResponse>.Failure(canAddError!);

            // Load tour (pricing, limits)
            var tour = await _tourRepository.FindOneAsync(t => t.Id == reservation.TourId, ct);
            if (tour is null)
                return ApplicationResult<AddGuestToReservationResponse>.Failure("تور مرتبط با رزرو یافت نشد");

            if (!tour.IsActive || !tour.IsRegistrationOpen(DateTime.UtcNow))
                return ApplicationResult<AddGuestToReservationResponse>.Failure("ثبت‌نام برای این تور مجاز نیست");

            // If determined type is Member, validate eligibility
            if (isMember && determinedParticipantType == ParticipantType.Member)
            {
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
                        : "این عضو شرایط لازم برای شرکت در این تور را ندارد";
                    return ApplicationResult<AddGuestToReservationResponse>.Failure(errorMessage);
                }
            }

            // Per-tour uniqueness of NationalNumber across all reservations (Draft/Held/Confirmed that are not voided)
            var existingInTour = await _reservationRepository
                .GetByTourIdAndNationalNumberAsync(tour.Id, request.Guest.NationalNumber, ct);

            // Exclude current reservation self-duplicates only if not present
            var duplicateInOtherReservation = existingInTour.Any(r => r.Id != reservation.Id && r.IsActive());
            if (duplicateInOtherReservation)
                return ApplicationResult<AddGuestToReservationResponse>.Failure("این کد ملی قبلاً برای این تور ثبت شده است");

            // Not duplicate inside current reservation
            if (reservation.Participants.Any(p => p.NationalNumber == request.Guest.NationalNumber))
                return ApplicationResult<AddGuestToReservationResponse>.Failure("این شرکت‌کننده قبلاً به همین رزرو اضافه شده است");

                var age = CalculateAge(request.Guest.BirthDate);
                if (tour.MinAge.HasValue && age < tour.MinAge.Value)
                    return ApplicationResult<AddGuestToReservationResponse>.Failure($"حداقل سن مجاز برای این تور {tour.MinAge} سال است");
                if (tour.MaxAge.HasValue && age > tour.MaxAge.Value)
                    return ApplicationResult<AddGuestToReservationResponse>.Failure($"حداکثر سن مجاز برای این تور {tour.MaxAge} سال است");

            // Tour-level limits: MaxParticipants and reservation-level MaxGuestsPerReservation
            var currentTotalParticipants = tour.GetConfirmedReservationCount() + tour.GetPendingReservationCount();
            if (currentTotalParticipants >= tour.MaxParticipants)
                return ApplicationResult<AddGuestToReservationResponse>.Failure("ظرفیت تور تکمیل شده است");

            if (!tour.CanAddGuestToReservation(reservation))
            {
                var maxGuests = tour.MaxGuestsPerReservation ?? 0;
                return ApplicationResult<AddGuestToReservationResponse>.Failure($"حداکثر تعداد مهمان مجاز برای هر رزرو {maxGuests} نفر است");
            }

            // Pricing - Use the auto-determined participant type
            var participantTypeEnum = determinedParticipantType;
            
            var pricingList = tour.GetActivePricing();
            var participantPricing = pricingList.FirstOrDefault(p => p.ParticipantType == participantTypeEnum);
            var participantTypeText = participantTypeEnum == ParticipantType.Member ? "عضو" : "مهمان";
            if (participantPricing is null)
                return ApplicationResult<AddGuestToReservationResponse>.Failure($"قیمت‌گذاری برای {participantTypeText} یافت نشد");

            var requiredPrice = participantPricing.GetEffectivePrice(); // Money

            // Create Participant aggregate
            var participant = new Participant(
                reservationId: reservation.Id,
                firstName: request.Guest.FirstName,
                lastName: request.Guest.LastName,
                nationalNumber: request.Guest.NationalNumber,
                phoneNumber: request.Guest.PhoneNumber,
                participantType: participantTypeEnum,
                requiredAmount: requiredPrice,
                email: request.Guest.Email,
                birthDate: request.Guest.BirthDate,
                emergencyContactName: request.Guest.EmergencyContactName,
                emergencyContactPhone: request.Guest.EmergencyContactPhone,
                notes: request.Guest.Notes);

            // Transactional add
            await _unitOfWork.BeginAsync(ct);
            reservation.AddParticipant(participant);

            // Persist
            await _unitOfWork.SaveChangesAsync(ct);
            await _unitOfWork.CommitAsync(ct);

            _logger.LogInformation("Added {Type} {Name} to reservation {ReservationId}",
                participantTypeText, $"{participant.FirstName} {participant.LastName}", reservation.Id);

            // Recompute totals using Money to avoid unit mistakes
            pricingList = tour.GetActivePricing();
            var guestPrice  = pricingList.FirstOrDefault(p => p.ParticipantType == ParticipantType.Guest)?.GetEffectivePrice()  ?? Money.Zero;
            var memberPrice = pricingList.FirstOrDefault(p => p.ParticipantType == ParticipantType.Member)?.GetEffectivePrice() ?? Money.Zero;

            var memberCount = reservation.Participants.Count(p => p.ParticipantType == ParticipantType.Member);
            var guestCount  = reservation.Participants.Count(p => p.ParticipantType == ParticipantType.Guest);

            var updatedTotal = Money.FromRials(memberCount * memberPrice.AmountRials + guestCount * guestPrice.AmountRials);

            var response = new AddGuestToReservationResponse
            {
                ParticipantId     = participant.Id,
                ReservationId      = reservation.Id,
                CapacityId = reservation.CapacityId,
            };

            return ApplicationResult<AddGuestToReservationResponse>.Success(response);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency while adding guest. ReservationId={ReservationId}", request.ReservationId);
            try { await _unitOfWork.RollbackAsync(ct); } catch { /* ignore */ }
            return ApplicationResult<AddGuestToReservationResponse>.Failure("تداخل همزمانی رخ داد. دوباره تلاش کنید");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while adding guest to reservation {ReservationId}", request.ReservationId);
            try { await _unitOfWork.RollbackAsync(ct); } catch { /* ignore */ }
            return ApplicationResult<AddGuestToReservationResponse>.Failure("خطا در اضافه کردن مهمان به رزرو");
        }
    }

    private static int CalculateAge(DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age)) age--;
        return age;
    }
}
