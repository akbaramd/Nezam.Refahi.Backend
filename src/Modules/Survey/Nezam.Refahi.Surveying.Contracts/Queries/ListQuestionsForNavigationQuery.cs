using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Queries;

/// <summary>
/// Query to get list of questions for navigation
/// </summary>
public record ListQuestionsForNavigationQuery(Guid SurveyId, Guid ResponseId, bool IncludeBackNavigation = false) : IRequest<ApplicationResult<QuestionsNavigationResponse>>;

/// <summary>
/// Response containing questions for navigation
/// </summary>
public class QuestionsNavigationResponse
{
    public Guid ResponseId { get; set; }
    public Guid SurveyId { get; set; }
    public int AttemptNumber { get; set; }
    
    // Navigation settings
    public bool AllowBackNavigation { get; set; }
    public bool IncludeBackNavigation { get; set; }
    
    // Questions list
    public List<NavigationQuestionDto> Questions { get; set; } = new();
    
    // Current position
    public Guid? CurrentQuestionId { get; set; }
    public int CurrentQuestionNumber { get; set; }
    public int TotalQuestions { get; set; }
    public double ProgressPercentage { get; set; }
}

/// <summary>
/// Question for navigation
/// </summary>
public class NavigationQuestionDto
{
    public Guid QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string QuestionKind { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsRequired { get; set; }
    public bool IsAnswered { get; set; }
    public bool IsComplete { get; set; }
    public bool IsCurrent { get; set; }
    public DateTime? LastAnsweredAt { get; set; }
}
