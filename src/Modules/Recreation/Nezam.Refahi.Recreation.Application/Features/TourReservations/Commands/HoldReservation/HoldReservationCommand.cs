using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.HoldReservation;

/// <summary>
/// Command to hold a draft reservation
/// Validates capacity, member capabilities, calculates pricing, creates snapshots, and transitions to OnHold status
/// </summary>
public class HoldReservationCommand : IRequest<ApplicationResult<HoldReservationResponse>>
{
    /// <summary>
    /// The ID of the draft reservation to hold
    /// </summary>
    public Guid ReservationId { get; set; }

    public HoldReservationCommand(Guid reservationId)
    {
        ReservationId = reservationId;
    }

    public HoldReservationCommand() { }
}
