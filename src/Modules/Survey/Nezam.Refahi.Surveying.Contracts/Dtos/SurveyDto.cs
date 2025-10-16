namespace Nezam.Refahi.Surveying.Contracts.Dtos;

/// <summary>
/// Survey data transfer object for client consumption with rich properties and Persian text
/// </summary>
public class SurveyDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // State information with Persian text
    public string State { get; set; } = string.Empty;
    public string StateText { get; set; } = string.Empty;
    
    // Timing information
    public DateTime? StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    
    // Survey configuration
    public bool IsAnonymous { get; set; }
    public bool IsStructureFrozen { get; set; }
    
    // Participation policy with Persian descriptions
    public int MaxAttemptsPerMember { get; set; }
    public string MaxAttemptsText { get; set; } = string.Empty;
    public bool AllowMultipleSubmissions { get; set; }
    public string AllowMultipleSubmissionsText { get; set; } = string.Empty;
    public int? CoolDownSeconds { get; set; }
    public string? CoolDownText { get; set; }
    public bool AllowBackNavigation { get; set; }
    public string AllowBackNavigationText { get; set; } = string.Empty;
    
    // Audience filter
    public string? AudienceFilter { get; set; }
    public bool HasAudienceFilter { get; set; }
    
    // Related data
    public List<QuestionDto> Questions { get; set; } = new();
    public List<SurveyFeatureDto> Features { get; set; } = new();
    public List<SurveyCapabilityDto> Capabilities { get; set; } = new();
    
    // User's response information (if applicable)
    public ResponseDto? UserLastResponse { get; set; }
    public List<ResponseDto> UserResponses { get; set; } = new();
    public bool HasUserResponse { get; set; }
    public bool CanUserParticipate { get; set; }
    public bool CanParticipate { get; set; } // Legacy property for backward compatibility
    public string? ParticipationMessage { get; set; }
    public int UserAttemptCount { get; set; }
    public int RemainingAttempts { get; set; }
    
    // Statistics
    public int TotalQuestions { get; set; }
    public int RequiredQuestions { get; set; }
    public int ResponseCount { get; set; }
    public int UniqueParticipantCount { get; set; }
    public bool IsAcceptingResponses { get; set; }
    public string IsAcceptingResponsesText { get; set; } = string.Empty;
    
    // Time-related information
    public TimeSpan? Duration { get; set; }
    public string? DurationText { get; set; }
    public TimeSpan? TimeRemaining { get; set; }
    public string? TimeRemainingText { get; set; }
    public bool IsExpired { get; set; }
    public bool IsScheduled { get; set; }
    public bool IsActive { get; set; }
    
    // Progress information for user
    public decimal? UserCompletionPercentage { get; set; }
    public int UserAnsweredQuestions { get; set; }
    public bool UserHasCompletedSurvey { get; set; }
}
