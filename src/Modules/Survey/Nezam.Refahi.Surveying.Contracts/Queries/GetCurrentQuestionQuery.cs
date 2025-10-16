using MediatR;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Queries;

/// <summary>
/// Query to get current question for a specific response
/// </summary>
public record GetCurrentQuestionQuery(Guid SurveyId, Guid ResponseId, int? RepeatIndex = null) : IRequest<ApplicationResult<CurrentQuestionResponse>>;

/// <summary>
/// Response containing current question information
/// </summary>
public class CurrentQuestionResponse
{
    public Guid QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string QuestionKind { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsRequired { get; set; }
    public bool IsAnswered { get; set; }
    public bool IsComplete { get; set; }
    
    // Repeatable question info
    public RepeatPolicyDto? RepeatPolicy { get; set; }
    public int RepeatIndex { get; set; } = 1;
    public bool IsRepeatAnswered { get; set; }
    public bool IsLastRepeat { get; set; }
    public int AnsweredRepeats { get; set; }
    public int? MaxRepeats { get; set; }
    public bool CanAddMoreRepeats { get; set; }
    
    // Question options
    public List<Contracts.Dtos.QuestionOptionDto> Options { get; set; } = new();
    
    // User's answer (if exists)
    public string? TextAnswer { get; set; }
    public List<Guid> SelectedOptionIds { get; set; } = new();
    
    // Navigation info
    public bool IsFirstQuestion { get; set; }
    public bool IsLastQuestion { get; set; }
    public int CurrentQuestionNumber { get; set; }
    public int TotalQuestions { get; set; }
    public double ProgressPercentage { get; set; }
    
    // Response info
    public Guid ResponseId { get; set; }
    public int AttemptNumber { get; set; }
    public bool AllowBackNavigation { get; set; }
}

