using MediatR;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Queries;

/// <summary>
/// Query to get response progress information
/// </summary>
public record GetResponseProgressQuery(Guid SurveyId, Guid ResponseId) : IRequest<ApplicationResult<ResponseProgressResponse>>;

/// <summary>
/// Response containing progress information
/// </summary>
public class ResponseProgressResponse
{
    public Guid ResponseId { get; set; }
    public Guid SurveyId { get; set; }
    public int AttemptNumber { get; set; }
    public string AttemptStatus { get; set; } = string.Empty; // Active, Submitted, Canceled, Expired
    public string ResponseStatus { get; set; } = string.Empty; // New response status
    public string ResponseStatusText { get; set; } = string.Empty; // Persian text for response status
    
    // Progress statistics
    public int TotalQuestions { get; set; }
    public int AnsweredQuestions { get; set; }
    public int RequiredQuestions { get; set; }
    public int AnsweredRequiredQuestions { get; set; }
    public double CompletionPercentage { get; set; }
    public double RequiredCompletionPercentage { get; set; }
    
    // Status
    public bool IsComplete { get; set; }
    public bool IsSubmitted { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public TimeSpan? TimeSpent { get; set; }
    
    // Navigation
    public bool AllowBackNavigation { get; set; }
    public List<QuestionProgressDto> QuestionsProgress { get; set; } = new();
    public List<RepeatableQuestionProgressDto> Repeatables { get; set; } = new();
}

/// <summary>
/// Individual question progress
/// </summary>
public class QuestionProgressDto
{
    public Guid QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsRequired { get; set; }
    public bool IsAnswered { get; set; }
    public bool IsComplete { get; set; }
    public DateTime? LastAnsweredAt { get; set; }
    
    // User answer data
    public string? UserTextAnswer { get; set; }
    public List<Guid> UserSelectedOptionIds { get; set; } = new();
    public List<string> UserSelectedOptionValues { get; set; } = new();
    public int RepeatIndex { get; set; } = 1;
}

/// <summary>
/// Repeatable question progress
/// </summary>
public class RepeatableQuestionProgressDto
{
    public Guid QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public RepeatPolicyDto RepeatPolicy { get; set; } = new();
    public int AnsweredRepeats { get; set; }
    public int? RequiredRepeats { get; set; }
    public bool CanAddMoreRepeats { get; set; }
    public List<int> AnsweredRepeatIndices { get; set; } = new();
    
    // User answer data for each repeat
    public List<RepeatableAnswerDto> RepeatAnswers { get; set; } = new();
}

/// <summary>
/// Answer data for a specific repeat of a repeatable question
/// </summary>
public class RepeatableAnswerDto
{
    public int RepeatIndex { get; set; }
    public string? UserTextAnswer { get; set; }
    public List<Guid> UserSelectedOptionIds { get; set; } = new();
    public List<string> UserSelectedOptionValues { get; set; } = new();
    public bool IsAnswered { get; set; }
}
