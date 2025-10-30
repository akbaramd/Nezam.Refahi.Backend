// =======================
// Handler (pricing-only)
// =======================
using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Queries.GetReservationPricing;

public sealed class GetReservationPricingQueryHandler
    : IRequestHandler<GetReservationPricingQuery, ApplicationResult<ReservationPricingResponse>>
{
    private readonly ITourReservationRepository _reservationRepository;
    private readonly ILogger<GetReservationPricingQueryHandler> _logger;

    public GetReservationPricingQueryHandler(
        ITourReservationRepository reservationRepository,
        ILogger<GetReservationPricingQueryHandler> logger)
    {
        _reservationRepository = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult<ReservationPricingResponse>> Handle(
        GetReservationPricingQuery request,
        CancellationToken ct)
    {
        if (request.ReservationId == Guid.Empty)
            return ApplicationResult<ReservationPricingResponse>.Failure("شناسه رزرو معتبر نیست");

        try
        {
            _logger.LogInformation("Calculating pricing for reservation {ReservationId}", request.ReservationId);

            // Expect repository to hydrate Participants (prefer an include-enabled method; adjust if needed)
            var reservation = await _reservationRepository.GetByIdAsync(request.ReservationId, ct);
            if (reservation is null)
                return ApplicationResult<ReservationPricingResponse>.Failure("رزرو یافت نشد");

            // Project participant pricing (null-safe)
            var participants = reservation.Participants.Select(p =>
            {
                var required = p.RequiredAmount?.AmountRials ?? 0m;
                var paid     = p.PaidAmount?.AmountRials ?? 0m;
                var remain   = p.RemainingAmount?.AmountRials ?? Math.Max(0m, required - paid);
                var fully    = remain == 0m && required >= 0m;

                return new ParticipantPricingDto
                {
                    ParticipantId   = p.Id,
                    ParticipantType = p.ParticipantType.ToString(),
                    RequiredAmount  = required,
                    PaidAmount      = paid,
                    RemainingAmount = remain,
                    IsFullyPaid     = fully
                };
            }).ToList();

            var totalRequired  = participants.Sum(x => x.RequiredAmount);
            var totalPaid      = participants.Sum(x => x.PaidAmount);
            var totalRemaining = participants.Sum(x => x.RemainingAmount);
            var allPaid        = totalRemaining == 0m;

            var response = new ReservationPricingResponse
            {
                ReservationId         = reservation.Id,
                TotalRequiredAmount   = totalRequired,
                TotalPaidAmount       = totalPaid,
                TotalRemainingAmount  = totalRemaining,
                IsFullyPaid           = allPaid,
                Participants          = participants
            };

            _logger.LogInformation(
                "Pricing computed. ReservationId={ReservationId}, Required={Required}, Paid={Paid}, Remaining={Remaining}, FullyPaid={Fully}",
                response.ReservationId, response.TotalRequiredAmount, response.TotalPaidAmount, response.TotalRemainingAmount, response.IsFullyPaid);

            return ApplicationResult<ReservationPricingResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to calculate pricing for reservation {ReservationId}", request.ReservationId);
            return ApplicationResult<ReservationPricingResponse>.Failure("خطا در محاسبه قیمت رزرو");
        }
    }
}
