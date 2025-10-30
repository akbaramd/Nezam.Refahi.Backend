using System.Text.Json.Serialization;

namespace Nezam.Refahi.Recreation.Domain.Enums;

/// <summary>
/// Represents the status of a tour
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TourDifficulty
{
    /// <summary>
    /// Tour is draft and not published yet
    /// </summary>
    Normal = 0,

    /// <summary>
    /// Tour is published but registration is not yet open (Pre-Open Window)
    /// </summary>
    Light = 1,

    /// <summary>
    /// Tour registration is open and available for booking
    /// </summary>
    High = 2,

    /// <summary>
    /// Tour registration is closed but tour hasn't started yet
    /// </summary>
    VeryHigh = 3,

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