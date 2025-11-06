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
    /// Survey is published and available for participation
    /// </summary>
    Published = 1,

    /// <summary>
    /// Survey has been completed (all responses collected)
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Survey has been archived for historical purposes
    /// </summary>
    Archived = 3
}
