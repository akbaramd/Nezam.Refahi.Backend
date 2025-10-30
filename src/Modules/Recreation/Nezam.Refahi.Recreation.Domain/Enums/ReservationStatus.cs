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
    /// Reservation is on hold for a short time window (e.g., 15 minutes)
    /// </summary>
    OnHold = 1,

    /// <summary>
    /// Processing in progress; awaiting confirmation
    /// </summary>
    PendingConfirmation = 2,

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
    /// Processing failed during confirmation workflow
    /// </summary>
    ProcessingFailed = 7,

    /// <summary>
    /// Post-cancellation processing in progress
    /// </summary>
    CancellationProcessing = 8,

    /// <summary>
    /// Post-cancellation processing completed
    /// </summary>
    CancellationProcessed = 9,

    /// <summary>
    /// User is waitlisted for capacity; needed for fair distribution and automated promotion
    /// </summary>
    Waitlisted = 10,

    /// <summary>
    /// Cancellation has been requested and is awaiting processing; prevents race conditions with callbacks
    /// </summary>
    CancellationRequested = 11,

    /// <summary>
    /// Request to modify participants/room/service that requires operator review
    /// </summary>
    AmendmentRequested = 12,

    /// <summary>
    /// No-show at tour start; essential for penalties and KPIs
    /// </summary>
    NoShow = 13,

    /// <summary>
    /// Reservation rejected by operator/eligibility rules before confirmation
    /// </summary>
    Rejected = 14
}