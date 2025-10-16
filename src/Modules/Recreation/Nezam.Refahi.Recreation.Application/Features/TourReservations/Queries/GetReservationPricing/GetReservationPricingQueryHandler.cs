using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Recreation.Application.Dtos;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Queries.GetReservationPricing;

public class GetReservationPricingQueryHandler
    : IRequestHandler<GetReservationPricingQuery, ApplicationResult<ReservationPricingResponse>>
{
    private readonly ITourReservationRepository _reservationRepository;
    private readonly ILogger<GetReservationPricingQueryHandler> _logger;

    public GetReservationPricingQueryHandler(
        ITourReservationRepository reservationRepository,
        ILogger<GetReservationPricingQueryHandler> logger)
    {
        _reservationRepository = reservationRepository;
        _logger = logger;
    }

    public async Task<ApplicationResult<ReservationPricingResponse>> Handle(
        GetReservationPricingQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting pricing for reservation {Identifier}",
                request.ReservationIdentifier);

            // Try to find reservation by ID first, then by tracking code
            Domain.Entities.TourReservation? reservation = null;

            if (Guid.TryParse(request.ReservationIdentifier, out var reservationId))
            {
                reservation = await _reservationRepository.FindOneAsync(x=>x.Id == reservationId, cancellationToken:cancellationToken);
            }

            reservation ??= await _reservationRepository.GetByTrackingCodeAsync(
                request.ReservationIdentifier, cancellationToken:cancellationToken);

            if (reservation == null)
            {
                return ApplicationResult<ReservationPricingResponse>.Failure("رزرو مورد نظر یافت نشد");
            }

            // Calculate pricing for each participant
            var participantPricing = reservation.Participants.Select(participant =>
                new ParticipantPricingDto
                {
                    ParticipantId = participant.Id,
                    FullName = participant.FullName,
                    NationalNumber = participant.NationalNumber,
                    ParticipantType = participant.ParticipantType.ToString(),
                    RequiredAmount = participant.RequiredAmount.AmountRials,
                    PaidAmount = participant.PaidAmount?.AmountRials ?? 0,
                    RemainingAmount = participant.RemainingAmount.AmountRials,
                    IsFullyPaid = participant.IsFullyPaid,
                    PaymentDate = participant.PaymentDate,
                    RegistrationDate = participant.RegistrationDate
                }).ToList();

            // Calculate totals
            var totalRequired = participantPricing.Sum(p => p.RequiredAmount);
            var totalPaid = participantPricing.Sum(p => p.PaidAmount);
            var totalRemaining = participantPricing.Sum(p => p.RemainingAmount);

            // Get tour details for enhanced response
            var tour = reservation.Tour;
            
            var response = new ReservationPricingResponse
            {
                ReservationId = reservation.Id,
                TrackingCode = reservation.TrackingCode,
                TourTitle = tour?.Title ?? "نامشخص",
                TourStart = tour?.TourStart,
                TourEnd = tour?.TourEnd,
                ReservationDate = reservation.ReservationDate,
                ExpiryDate = reservation.ExpiryDate,
                ConfirmationDate = reservation.ConfirmationDate,
                ParticipantPricing = participantPricing,
                TotalRequiredAmount = totalRequired,
                TotalPaidAmount = totalPaid,
                TotalRemainingAmount = totalRemaining,
                IsFullyPaid = totalRemaining == 0,
                PaymentDeadline = reservation.ExpiryDate,
                Status = reservation.Status.ToString(),
                IsExpired = reservation.IsExpired(),
                IsPending = reservation.IsPending(),
                IsConfirmed = reservation.IsConfirmed(),
                ParticipantCount = reservation.GetParticipantCount(),
                MainParticipantCount = participantPricing.Count(p => p.ParticipantType == "Member"),
                GuestParticipantCount = participantPricing.Count(p => p.ParticipantType == "Guest")
            };

            _logger.LogInformation("Successfully retrieved pricing for reservation {ReservationId}. " +
                                 "Total Required: {TotalRequired}, Total Paid: {TotalPaid}, Remaining: {Remaining}",
                reservation.Id, totalRequired, totalPaid, totalRemaining);

            return ApplicationResult<ReservationPricingResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting pricing for reservation {Identifier}",
                request.ReservationIdentifier);
            return ApplicationResult<ReservationPricingResponse>.Failure("خطا در دریافت اطلاعات قیمت رزرو");
        }
    }
}