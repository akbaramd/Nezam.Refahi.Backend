using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Queries;

/// <summary>
/// Query to get previous questions from a specific index
/// </summary>
public class GetPreviousQuestionsQuery : IRequest<ApplicationResult<GetPreviousQuestionsResponse>>
{
    /// <summary>
    /// Survey ID
    /// </summary>
    public Guid SurveyId { get; set; }

    /// <summary>
    /// Current question index (0-based) - questions before this index will be returned
    /// </summary>
    public int CurrentQuestionIndex { get; set; }

    /// <summary>
    /// Maximum number of previous questions to return (default: 10)
    /// </summary>
    public int MaxCount { get; set; } = 10;

    /// <summary>
    /// Member ID for user-specific data
    /// </summary>
    public string? UserNationalNumber { get; set; }

    /// <summary>
    /// Response ID if user is answering a specific response
    /// </summary>
    public Guid? ResponseId { get; set; }

    /// <summary>
    /// Include user's answers for these questions
    /// </summary>
    public bool IncludeUserAnswers { get; set; } = true;

    /// <summary>
    /// Include navigation information
    /// </summary>
    public bool IncludeNavigationInfo { get; set; } = true;
}
