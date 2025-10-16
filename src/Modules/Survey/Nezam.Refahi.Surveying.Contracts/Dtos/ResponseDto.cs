namespace Nezam.Refahi.Surveying.Contracts.Dtos;

/// <summary>
/// Response data transfer object for client consumption with rich properties and Persian text
/// </summary>
public class ResponseDto
{
    public Guid Id { get; set; }
    public Guid SurveyId { get; set; }
    public int AttemptNumber { get; set; }
 
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ExpiredAt { get; set; }
    public DateTime? CanceledAt { get; set; }
    
    // Participant info with Persian text
    public string ParticipantDisplayName { get; set; } = string.Empty;
    public string ParticipantShortIdentifier { get; set; } = string.Empty;
    public bool IsAnonymous { get; set; }
    public string IsAnonymousText { get; set; } = string.Empty;
    
    // Response status with Persian text
    public string AttemptStatus { get; set; } = string.Empty;
    public string AttemptStatusText { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsSubmitted { get; set; }
    public bool IsExpired { get; set; }
    public bool IsCanceled { get; set; }
    
    // Answer data
    public List<QuestionAnswerDto> QuestionAnswers { get; set; } = new();
    public List<QuestionAnswerDto> LastAnswers { get; set; } = new(); // Latest answers for each question
    
    // Progress information
    public int TotalQuestions { get; set; }
    public int AnsweredQuestions { get; set; }
    public int RequiredQuestions { get; set; }
    public int RequiredAnsweredQuestions { get; set; }
    public bool IsComplete { get; set; }
    public string IsCompleteText { get; set; } = string.Empty;
    public decimal CompletionPercentage { get; set; }
    public string CompletionPercentageText { get; set; } = string.Empty;
    
    // Time information
    public TimeSpan? ResponseDuration { get; set; }
    public string? ResponseDurationText { get; set; }
    public TimeSpan? TimeToExpire { get; set; }
    public string? TimeToExpireText { get; set; }
    
    // Survey information
    public string? SurveyTitle { get; set; }
    public string? SurveyDescription { get; set; }
    public bool CanContinue { get; set; }
    public bool CanSubmit { get; set; }
    public bool CanCancel { get; set; }
    public string? NextActionText { get; set; }
    
    // Validation and errors
    public List<string> ValidationErrors { get; set; } = new();
    public bool HasValidationErrors { get; set; }
    public string? ValidationMessage { get; set; }
}
