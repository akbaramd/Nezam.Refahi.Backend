using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Recreation.Application.Services;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.AddGuest;

public class AddGuestToReservationCommandHandler
    : IRequestHandler<AddGuestToReservationCommand, ApplicationResult<AddGuestToReservationResponse>>
{
    private readonly ITourReservationRepository _reservationRepository;
    private readonly ITourRepository _tourRepository;
    private readonly IRecreationUnitOfWork _unitOfWork;
    private readonly ParticipantValidationService _participantValidationService;
    private readonly ILogger<AddGuestToReservationCommandHandler> _logger;

    public AddGuestToReservationCommandHandler(
        ITourReservationRepository reservationRepository,
        ITourRepository tourRepository,
        IRecreationUnitOfWork unitOfWork,
        ParticipantValidationService participantValidationService,
        ILogger<AddGuestToReservationCommandHandler> logger)
    {
        _reservationRepository = reservationRepository;
        _tourRepository = tourRepository;
        _unitOfWork = unitOfWork;
        _participantValidationService = participantValidationService;
        _logger = logger;
    }

    public async Task<ApplicationResult<AddGuestToReservationResponse>> Handle(
        AddGuestToReservationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Adding participant {NationalNumber} to reservation {ReservationId}",
                request.Guest.NationalNumber, request.ReservationId);

            // Validate participant and check membership if needed
            var validationResult = await _participantValidationService.ValidateParticipantAsync(request.Guest, cancellationToken);
            if (!validationResult.IsValid)
            {
                return ApplicationResult<AddGuestToReservationResponse>.Failure(validationResult.ErrorMessage!);
            }

            // Get reservation with tour information
            var reservation = await _reservationRepository.FindOneAsync(x=>x.Id==request.ReservationId, cancellationToken:cancellationToken);
            if (reservation == null)
            {
                return ApplicationResult<AddGuestToReservationResponse>.Failure("رزرو مورد نظر یافت نشد");
            }

            // Check reservation status
            if (reservation.Status != ReservationStatus.Held)
            {
                return ApplicationResult<AddGuestToReservationResponse>.Failure("امکان اضافه کردن مهمان به رزرو تأیید شده یا لغو شده وجود ندارد");
            }

            // Check if reservation is expired
            if (reservation.IsExpired())
            {
                return ApplicationResult<AddGuestToReservationResponse>.Failure("رزرو منقضی شده است");
            }

            // Get tour information
            var tour = await _tourRepository.FindOneAsync(x=>x.Id==reservation.TourId, cancellationToken:cancellationToken);
            if (tour == null)
            {
                return ApplicationResult<AddGuestToReservationResponse>.Failure("تور مرتبط با رزرو یافت نشد");
            }

            // Check if tour still has available spots
            var currentTotalParticipants = tour.GetConfirmedReservationCount() + tour.GetPendingReservationCount();
            if (currentTotalParticipants >= tour.MaxParticipants)
            {
                return ApplicationResult<AddGuestToReservationResponse>.Failure("ظرفیت تور تکمیل شده است");
            }

            // Check if participant already exists in this reservation
            if (reservation.Participants.Any(p => p.NationalNumber == request.Guest.NationalNumber))
            {
                return ApplicationResult<AddGuestToReservationResponse>.Failure("این شرکت‌کننده قبلاً به رزرو اضافه شده است");
            }

           
                var age = CalculateAge(request.Guest.BirthDate);
                if (tour.MinAge.HasValue && age < tour.MinAge.Value)
                {
                    return ApplicationResult<AddGuestToReservationResponse>.Failure($"حداقل سن مجاز برای این تور {tour.MinAge} سال است");
                }
                if (tour.MaxAge.HasValue && age > tour.MaxAge.Value)
                {
                    return ApplicationResult<AddGuestToReservationResponse>.Failure($"حداکثر سن مجاز برای این تور {tour.MaxAge} سال است");
                }
           
           

            // Get pricing based on participant type
            var activePricing = tour.GetActivePricing();
            var participantPricing = activePricing.FirstOrDefault(p => p.ParticipantType == validationResult.ParticipantType);
            var participantTypeText = validationResult.ParticipantType == ParticipantType.Member ? "عضو" : "مهمان";
            if (participantPricing == null)
            {
                return ApplicationResult<AddGuestToReservationResponse>.Failure($"قیمت‌گذاری برای {participantTypeText} یافت نشد");
            }

            var requiredAmount = participantPricing.GetEffectivePrice();

            // Create participant with correct type and pricing
            var participant = new Participant(
                reservationId: reservation.Id,
                firstName: request.Guest.FirstName,
                lastName: request.Guest.LastName,
                nationalNumber: request.Guest.NationalNumber,
                phoneNumber: request.Guest.PhoneNumber,
                participantType: validationResult.ParticipantType,
                requiredAmount: requiredAmount,
                email: request.Guest.Email,
                birthDate: request.Guest.BirthDate,
                emergencyContactName: request.Guest.EmergencyContactName,
                emergencyContactPhone: request.Guest.EmergencyContactPhone,
                notes: request.Guest.Notes);

            // Add participant to reservation
            reservation.AddParticipant(participant);

            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

         
            _logger.LogInformation("Successfully added {ParticipantType} {ParticipantName} to reservation {ReservationId}",
                participantTypeText, $"{request.Guest.FirstName} {request.Guest.LastName}", request.ReservationId);

            // Calculate updated pricing
            activePricing = tour.GetActivePricing();
            var guestPricing = activePricing.FirstOrDefault(p => p.ParticipantType == ParticipantType.Guest);
            var memberPricing = activePricing.FirstOrDefault(p => p.ParticipantType == ParticipantType.Member);

            var memberParticipants = reservation.Participants.Count(p => p.ParticipantType == ParticipantType.Member);
            var guestParticipants = reservation.Participants.Count(p => p.ParticipantType == ParticipantType.Guest);

            var updatedTotalPrice = (memberParticipants * (memberPricing?.GetEffectivePrice().AmountRials ?? 0)) +
                                   (guestParticipants * (guestPricing?.GetEffectivePrice().AmountRials ?? 0));

            var response = new AddGuestToReservationResponse
            {
                ParticipantId = participant.Id,
                TrackingCode = reservation.TrackingCode,
                TotalParticipants = reservation.Participants.Count,
                UpdatedTotalPrice = updatedTotalPrice,
                GuestName = $"{request.Guest.FirstName} {request.Guest.LastName}"
            };

            return ApplicationResult<AddGuestToReservationResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding guest to reservation {ReservationId}",
                request.ReservationId);
            return ApplicationResult<AddGuestToReservationResponse>.Failure("خطا در اضافه کردن مهمان به رزرو");
        }
    }

    private static int CalculateAge(DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age))
            age--;
        return age;
    }
}