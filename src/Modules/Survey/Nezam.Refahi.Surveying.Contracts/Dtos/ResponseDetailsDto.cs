using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Dtos;

public class ResponseDetailsDto
{
    public Guid Id { get; set; }
    public Guid SurveyId { get; set; }
    public int AttemptNumber { get; set; }

    
    // Participant Information
    public ParticipantInfoDto Participant { get; set; } = new();
    
    // Survey Information
    public SurveyBasicInfoDto? Survey { get; set; }
    
    // Question Answers
    public List<QuestionAnswerDetailsDto> QuestionAnswers { get; set; } = new();
    
    // Response Statistics
    public ResponseStatisticsDto Statistics { get; set; } = new();
    
    // Response Status
    public ResponseStatusDto Status { get; set; } = new();
}

public class ParticipantInfoDto
{
    public Guid? MemberId { get; set; }
    public string? ParticipantHash { get; set; }
    public string? NationalCode { get; set; }
    public string? FullName { get; set; }
    public bool IsAnonymous { get; set; }
    public Dictionary<string, string>? DemographyData { get; set; }
}

public class SurveyBasicInfoDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string StateText { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsAnonymous { get; set; }
    public DateTime? StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public int TotalQuestions { get; set; }
    public int RequiredQuestions { get; set; }
}

public class QuestionAnswerDetailsDto
{
    public Guid Id { get; set; }
    public Guid ResponseId { get; set; }
    public Guid QuestionId { get; set; }
    public string? TextAnswer { get; set; }
    public List<Guid> SelectedOptionIds { get; set; } = new();
    public List<QuestionOptionDto> SelectedOptions { get; set; } = new();

    public bool IsAnswered { get; set; }
    public bool IsComplete { get; set; }
    
    // Question Details (if IncludeQuestionDetails is true)
    public QuestionDetailsDto? Question { get; set; }
}

public class ResponseStatisticsDto
{
    public int TotalQuestions { get; set; }
    public int AnsweredQuestions { get; set; }
    public int RequiredQuestions { get; set; }
    public int RequiredAnsweredQuestions { get; set; }
    public decimal CompletionPercentage { get; set; }
    public bool IsComplete { get; set; }
    public TimeSpan? TimeSpent { get; set; }
    public DateTime? FirstAnswerAt { get; set; }
    public DateTime? LastAnswerAt { get; set; }
}

public class ResponseStatusDto
{
    public string ResponseStatus { get; set; } = string.Empty; // New response status
    public string ResponseStatusText { get; set; } = string.Empty; // Persian text for response status
    public bool CanContinue { get; set; }
    public bool CanSubmit { get; set; }
    public bool IsSubmitted { get; set; }
    public string StatusMessage { get; set; } = string.Empty;
    public List<string> ValidationErrors { get; set; } = new();
}
