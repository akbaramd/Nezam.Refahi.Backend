using MediatR;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Queries.GetReservationDetail;

/// <summary>
/// Query to get detailed information about a tour reservation
/// </summary>
public class GetReservationDetailQuery : IRequest<ApplicationResult<ReservationDetailDto>>
{
    /// <summary>
    /// The ID of the reservation to retrieve
    /// </summary>
    public Guid ReservationId { get; set; }

    public string UserNationalNumber { get; set; } = string.Empty;

    public GetReservationDetailQuery(Guid reservationId, string userNationalNumber)
    {
        ReservationId = reservationId;
        UserNationalNumber = userNationalNumber;
    }
}