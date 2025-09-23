using System.Text.Json.Serialization;

namespace Nezam.Refahi.Recreation.Domain.Enums;

/// <summary>
/// Status of a tour reservation following explicit state machine pattern
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReservationStatus
{
    /// <summary>
    /// Draft state - reservation is being created (temporary state)
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Reservation is held/pending payment (15 minutes timeout)
    /// </summary>
    Held = 1,

    /// <summary>
    /// Payment is in progress
    /// </summary>
    Paying = 2,

    /// <summary>
    /// Reservation is confirmed and payment received
    /// </summary>
    Confirmed = 3,

    /// <summary>
    /// Reservation was cancelled by user
    /// </summary>
    Cancelled = 4,

    /// <summary>
    /// Reservation expired due to timeout
    /// </summary>
    Expired = 5,

    /// <summary>
    /// Reservation was cancelled by system (e.g., tour cancelled)
    /// </summary>
    SystemCancelled = 6,

    /// <summary>
    /// Payment failed after attempts
    /// </summary>
    PaymentFailed = 7,

    /// <summary>
    /// Refund is being processed
    /// </summary>
    Refunding = 8,

    /// <summary>
    /// Refund completed
    /// </summary>
    Refunded = 9,

    /// <summary>
    /// User is waitlisted for capacity; needed for fair distribution and automated promotion
    /// </summary>
    Waitlisted = 10,

    /// <summary>
    /// Cancellation requested in Paying or Confirmed status, but financial arbitration (PSP) not yet completed. Prevents race conditions with callbacks
    /// </summary>
    CancelRequested = 11,

    /// <summary>
    /// Request to modify participants/room/service that requires operator review
    /// </summary>
    AmendRequested = 12,

    /// <summary>
    /// No-show at tour start; essential for penalties and KPIs
    /// </summary>
    NoShow = 13,

    /// <summary>
    /// Reservation rejected by operator/eligibility rules before confirmation
    /// </summary>
    Rejected = 14
}