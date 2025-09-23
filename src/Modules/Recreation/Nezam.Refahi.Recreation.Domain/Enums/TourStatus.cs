using System.Text.Json.Serialization;

namespace Nezam.Refahi.Recreation.Domain.Enums;

/// <summary>
/// Represents the status of a tour
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TourStatus
{
    /// <summary>
    /// Tour is draft and not published yet
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Tour is published but registration is not yet open (Pre-Open Window)
    /// </summary>
    Scheduled = 1,

    /// <summary>
    /// Tour registration is open and available for booking
    /// </summary>
    RegistrationOpen = 2,

    /// <summary>
    /// Tour registration is closed but tour hasn't started yet
    /// </summary>
    RegistrationClosed = 3,

    /// <summary>
    /// Tour is in progress
    /// </summary>
    InProgress = 4,

    /// <summary>
    /// Tour has been completed
    /// </summary>
    Completed = 5,

    /// <summary>
    /// Tour has been cancelled
    /// </summary>
    Cancelled = 6,

    /// <summary>
    /// Tour is suspended temporarily
    /// </summary>
    Suspended = 7,

    /// <summary>
    /// Tour has been postponed (Reschedule without Cancel)
    /// </summary>
    Postponed = 8,

    /// <summary>
    /// Tour is operationally archived after complete reporting; prevents UI display and changes
    /// </summary>
    Archived = 9
}