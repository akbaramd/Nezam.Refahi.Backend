using MediatR;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Queries;

/// <summary>
/// Query to get specific question by ID for a response
/// </summary>
public record GetQuestionByIdQuery(Guid SurveyId, Guid ResponseId, Guid QuestionId, int? RepeatIndex = null) : IRequest<ApplicationResult<QuestionByIdResponse>>;

/// <summary>
/// Response containing specific question information
/// </summary>
public class QuestionByIdResponse
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
