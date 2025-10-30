using Nezam.Refahi.Surveying.Contracts.Dtos;

namespace Nezam.Refahi.Surveying.Contracts.Queries;

/// <summary>
/// Response for GetSpecificQuestionQuery
/// </summary>
public class GetSpecificQuestionResponse
{
    /// <summary>
    /// Survey basic information
    /// </summary>
    public SurveyBasicInfoDto Survey { get; set; } = null!;

    /// <summary>
    /// Current question details
    /// </summary>
    public QuestionDetailsDto CurrentQuestion { get; set; } = null!;

    /// <summary>
    /// Navigation information
    /// </summary>
    public QuestionNavigationDto Navigation { get; set; } = null!;

    /// <summary>
    /// User's answer for current question (if available)
    /// </summary>
    public QuestionAnswerDto? UserAnswer { get; set; }

    /// <summary>
    /// User's response status
    /// </summary>
    public UserResponseStatusDto? UserResponseStatus { get; set; }

    /// <summary>
    /// Question statistics (if requested)
    /// </summary>
    public QuestionStatisticsDto? Statistics { get; set; }
}
