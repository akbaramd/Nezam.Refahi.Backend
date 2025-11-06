using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.FinalizeReservation;

/// <summary>
/// Command to finalize a draft reservation
/// Validates all participants, creates pricing snapshots, creates bill, and holds the reservation
/// </summary>
public class FinalizeReservationCommand : IRequest<ApplicationResult<FinalizeReservationResponse>>
{
    /// <summary>
    /// The ID of the draft reservation to finalize
    /// </summary>
    public Guid ReservationId { get; set; }

    public FinalizeReservationCommand(Guid reservationId)
    {
        ReservationId = reservationId;
    }

    public FinalizeReservationCommand() { }
}

