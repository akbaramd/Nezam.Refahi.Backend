namespace Nezam.Refahi.Recreation.Domain.Enums;

/// <summary>
/// Represents the status of a tour
/// </summary>
public enum TourStatus
{
    /// <summary>
    /// Tour is draft and not published yet
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Tour is active and available for registration
    /// </summary>
    Active = 1,

    /// <summary>
    /// Tour registration is closed but tour hasn't started yet
    /// </summary>
    RegistrationClosed = 2,

    /// <summary>
    /// Tour is in progress
    /// </summary>
    InProgress = 3,

    /// <summary>
    /// Tour has been completed
    /// </summary>
    Completed = 4,

    /// <summary>
    /// Tour has been cancelled
    /// </summary>
    Cancelled = 5,

    /// <summary>
    /// Tour is suspended temporarily
    /// </summary>
    Suspended = 6
}