using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.ReactivateExpiredReservation;

/// <summary>
/// Command to reactivate an expired reservation if capacity is available
/// </summary>
public class ReactivateExpiredReservationCommand : IRequest<ApplicationResult<ReactivateExpiredReservationResponse>>
{
    /// <summary>
    /// The ID of the expired reservation to reactivate
    /// </summary>
    public Guid ReservationId { get; set; }

    /// <summary>
    /// Optional reason for reactivation
    /// </summary>
    public string? Reason { get; set; }



    public ReactivateExpiredReservationCommand(Guid reservationId, string? reason = null)
    {
        ReservationId = reservationId;
        Reason = reason;
    }

    public ReactivateExpiredReservationCommand() { }
}

/// <summary>
/// Response for reactivate expired reservation command
/// </summary>
public class ReactivateExpiredReservationResponse
{
    /// <summary>
    /// The ID of the reservation
    /// </summary>
    public Guid ReservationId { get; set; }

    /// <summary>
    /// Tracking code of the reservation
    /// </summary>
    public string TrackingCode { get; set; } = null!;

    /// <summary>
    /// Whether the reservation was successfully reactivated
    /// </summary>
    public bool WasReactivated { get; set; }

    /// <summary>
    /// Whether the reservation was deleted due to no capacity
    /// </summary>
    public bool WasDeleted { get; set; }

    /// <summary>
    /// New status after reactivation attempt
    /// </summary>
    public string? NewStatus { get; set; }

    /// <summary>
    /// New expiry date if reactivated
    /// </summary>
    public DateTime? NewExpiryDate { get; set; }

    /// <summary>
    /// Reason for reactivation or deletion
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Available capacity at time of reactivation attempt
    /// </summary>
    public int AvailableCapacity { get; set; }

    /// <summary>
    /// Required capacity for this reservation
    /// </summary>
    public int RequiredCapacity { get; set; }

    /// <summary>
    /// Date when reactivation was attempted
    /// </summary>
    public DateTime ProcessedDate { get; set; }
}
