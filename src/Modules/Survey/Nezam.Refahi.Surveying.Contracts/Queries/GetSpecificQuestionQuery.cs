using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Queries;

/// <summary>
/// Query to get a specific question by index with navigation capabilities
/// </summary>
public class GetSpecificQuestionQuery : IRequest<ApplicationResult<GetSpecificQuestionResponse>>
{
    /// <summary>
    /// Survey ID
    /// </summary>
    public Guid SurveyId { get; set; }

    /// <summary>
    /// Question index (0-based)
    /// </summary>
    public int QuestionIndex { get; set; }

    /// <summary>
    /// Member ID for user-specific data
    /// </summary>
    public string? UserNationalNumber { get; set; }

    /// <summary>
    /// Response ID if user is answering a specific response
    /// </summary>
    public Guid? ResponseId { get; set; }

    /// <summary>
    /// Include user's answers for this question
    /// </summary>
    public bool IncludeUserAnswers { get; set; } = true;

    /// <summary>
    /// Include navigation information (previous/next question)
    /// </summary>
    public bool IncludeNavigationInfo { get; set; } = true;

    /// <summary>
    /// Include question statistics
    /// </summary>
    public bool IncludeStatistics { get; set; } = false;
}
