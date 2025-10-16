namespace Nezam.Refahi.Surveying.Domain.Enums;

/// <summary>
/// Represents the status of a survey attempt/response
/// </summary>
public enum AttemptStatus
{
    /// <summary>
    /// Attempt is currently active and being answered
    /// </summary>
    Active = 0,

    /// <summary>
    /// Attempt has been submitted and is immutable
    /// </summary>
    Submitted = 1,

    /// <summary>
    /// Attempt has been canceled/abandoned
    /// </summary>
    Canceled = 2,

    /// <summary>
    /// Attempt has expired due to survey time window or organizational rules
    /// </summary>
    Expired = 3
}
