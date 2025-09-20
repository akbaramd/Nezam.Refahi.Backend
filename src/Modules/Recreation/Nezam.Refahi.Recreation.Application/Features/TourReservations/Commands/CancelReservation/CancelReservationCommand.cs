using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.CancelReservation;

/// <summary>
/// Command to cancel a tour reservation
/// </summary>
public class CancelReservationCommand : IRequest<ApplicationResult<CancelReservationResponse>>
{
    /// <summary>
    /// The ID of the reservation to cancel
    /// </summary>
    public Guid ReservationId { get; set; }

    /// <summary>
    /// Optional reason for cancellation
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Whether to permanently delete the reservation and participants from database
    /// If false, only marks as cancelled. If true, removes all data.
    /// </summary>
    public bool PermanentDelete { get; set; } = true;

    public CancelReservationCommand(Guid reservationId, string? reason = null, bool permanentDelete = true)
    {
        ReservationId = reservationId;
        Reason = reason;
        PermanentDelete = permanentDelete;
    }

    public CancelReservationCommand() { }
}

/// <summary>
/// Response for cancel reservation command
/// </summary>
public class CancelReservationResponse
{
    /// <summary>
    /// The ID of the cancelled reservation
    /// </summary>
    public Guid ReservationId { get; set; }

    /// <summary>
    /// Tracking code of the cancelled reservation
    /// </summary>
    public string TrackingCode { get; set; } = null!;

    /// <summary>
    /// Whether the reservation was permanently deleted
    /// </summary>
    public bool WasDeleted { get; set; }

    /// <summary>
    /// Number of participants that were removed
    /// </summary>
    public int ParticipantsRemoved { get; set; }

    /// <summary>
    /// Cancellation reason
    /// </summary>
    public string? CancellationReason { get; set; }

    /// <summary>
    /// Date when cancellation occurred
    /// </summary>
    public DateTime CancellationDate { get; set; }

    /// <summary>
    /// Amount that may be refunded (if applicable)
    /// </summary>
    public decimal? RefundableAmountRials { get; set; }
}