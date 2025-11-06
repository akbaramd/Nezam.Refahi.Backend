using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.ExpireReservation;

/// <summary>
/// Command to expire a reservation and cancel associated bills
/// This command is typically invoked when a reservation's expiry date has passed
/// </summary>
public class ExpireReservationCommand : IRequest<ApplicationResult<ExpireReservationResponse>>
{
    /// <summary>
    /// The ID of the reservation to expire
    /// </summary>
    public Guid ReservationId { get; set; }

    /// <summary>
    /// Optional reason for expiration
    /// </summary>
    public string? Reason { get; set; }

    public ExpireReservationCommand(Guid reservationId, string? reason = null)
    {
        ReservationId = reservationId;
        Reason = reason;
    }

    public ExpireReservationCommand() { }
}

/// <summary>
/// Response for expire reservation command
/// </summary>
public class ExpireReservationResponse
{
    /// <summary>
    /// The ID of the expired reservation
    /// </summary>
    public Guid ReservationId { get; set; }

    /// <summary>
    /// Tracking code of the expired reservation
    /// </summary>
    public string TrackingCode { get; set; } = null!;

    /// <summary>
    /// Previous status before expiration
    /// </summary>
    public string PreviousStatus { get; set; } = null!;

    /// <summary>
    /// Whether associated bill was cancelled
    /// </summary>
    public bool BillCancelled { get; set; }

    /// <summary>
    /// Bill ID if it was cancelled
    /// </summary>
    public Guid? BillId { get; set; }
}

