using MediatR;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Queries.GetReservationPricing;

/// <summary>
/// Query to get pricing details for a reservation
/// </summary>
public class GetReservationPricingQuery : IRequest<ApplicationResult<ReservationPricingResponse>>
{
    /// <summary>
    /// Reservation ID or tracking code
    /// </summary>
    public string ReservationIdentifier { get; init; } = null!;
}