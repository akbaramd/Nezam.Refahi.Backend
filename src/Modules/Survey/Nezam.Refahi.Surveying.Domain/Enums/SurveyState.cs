namespace Nezam.Refahi.Surveying.Domain.Enums;

/// <summary>
/// Survey state enumeration representing the lifecycle of a survey
/// </summary>
public enum SurveyState
{
    /// <summary>
    /// Survey is being created and configured
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Survey is scheduled to start at a specific time
    /// </summary>
    Scheduled = 1,

    /// <summary>
    /// Survey is currently active and accepting responses
    /// </summary>
    Active = 2,

    /// <summary>
    /// Survey is temporarily paused (not accepting new responses but existing ones can continue)
    /// </summary>
    Paused = 3,

    /// <summary>
    /// Survey has ended and is no longer accepting responses
    /// </summary>
    Closed = 4,

    /// <summary>
    /// Survey has been archived for historical purposes
    /// </summary>
    Archived = 5
}
