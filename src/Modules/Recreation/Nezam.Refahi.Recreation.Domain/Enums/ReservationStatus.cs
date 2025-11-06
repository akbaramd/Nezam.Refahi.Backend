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

    OnHold = 4,



    /// <summary>
    /// Reservation is fully confirmed
    /// </summary>
    Confirmed = 3,

    /// <summary>
    /// Reservation was cancelled by user
    /// </summary>
    Cancelled = 6,

    /// <summary>
    /// Reservation expired due to timeout
    /// </summary>
    Expired = 7,

    /// <summary>
    /// Reservation was cancelled by system (e.g., tour cancelled)
    /// </summary>
    SystemCancelled = 8,

    /// <summary>
    /// Processing failed during confirmation workflow
    /// </summary>
    ProcessingFailed = 9,

    /// <summary>
    /// Post-cancellation processing in progress
    /// </summary>
    CancellationProcessing = 10,

    /// <summary>
    /// Post-cancellation processing completed
    /// </summary>
    CancellationProcessed = 11,

    /// <summary>
    /// User is waitlisted for capacity; needed for fair distribution and automated promotion
    /// </summary>
    Waitlisted = 12,

    /// <summary>
    /// Cancellation has been requested and is awaiting processing; prevents race conditions with callbacks
    /// </summary>
    CancellationRequested = 13,

    /// <summary>
    /// Request to modify participants/room/service that requires operator review
    /// </summary>
    AmendmentRequested = 14,

    /// <summary>
    /// No-show at tour start; essential for penalties and KPIs
    /// </summary>
    NoShow = 15,

    /// <summary>
    /// Reservation rejected by operator/eligibility rules before confirmation
    /// </summary>
    Rejected = 16
}