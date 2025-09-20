namespace Nezam.Refahi.Recreation.Domain.Enums;

/// <summary>
/// Status of a tour reservation following explicit state machine pattern
/// </summary>
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
    Refunded = 9
}