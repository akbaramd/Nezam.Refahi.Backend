namespace Nezam.Refahi.Surveying.Contracts.Dtos;

/// <summary>
/// Detailed Survey data transfer object for single survey queries
/// Contains comprehensive information including all related data
/// </summary>
public class SurveyDetailsDto
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
    public int StructureVersion { get; set; }
    
    // Participation policy with Persian descriptions
    public ParticipationPolicyDto ParticipationPolicy { get; set; } = new();
    
    // Audience filter
    public AudienceFilterDto? AudienceFilter { get; set; }
    
    // Related data - Detailed versions
    public List<QuestionDetailsDto> Questions { get; set; } = new();
    public List<SurveyFeatureDto> Features { get; set; } = new();
    public List<SurveyCapabilityDto> Capabilities { get; set; } = new();
    
    // User's response information (if applicable)
    public ResponseDetailsDto? UserLastResponse { get; set; }
    public List<ResponseDetailsDto> UserResponses { get; set; } = new();
    public UserParticipationInfoDto UserParticipation { get; set; } = new();
    
    // Comprehensive statistics
    public SurveyStatisticsDto Statistics { get; set; } = new();
    
    // Time-related information
    public SurveyTimingDto Timing { get; set; } = new();
    
    // Survey status and capabilities
    public SurveyStatusDto Status { get; set; } = new();
}

/// <summary>
/// Participation policy details
/// </summary>
public class ParticipationPolicyDto
{
    public int MaxAttemptsPerMember { get; set; }
    public string MaxAttemptsText { get; set; } = string.Empty;
    public bool AllowMultipleSubmissions { get; set; }
    public string AllowMultipleSubmissionsText { get; set; } = string.Empty;
    public int? CoolDownSeconds { get; set; }
    public string? CoolDownText { get; set; }
    public bool AllowBackNavigation { get; set; }
    public string AllowBackNavigationText { get; set; } = string.Empty;
    public bool RequireAllQuestions { get; set; }
    public string RequireAllQuestionsText { get; set; } = string.Empty;
}

/// <summary>
/// Audience filter details
/// </summary>
public class AudienceFilterDto
{
    public string FilterType { get; set; } = string.Empty;
    public string FilterTypeText { get; set; } = string.Empty;
    public List<string> RequiredFeatures { get; set; } = new();
    public List<string> RequiredCapabilities { get; set; } = new();
    public List<string> ExcludedFeatures { get; set; } = new();
    public List<string> ExcludedCapabilities { get; set; } = new();
    public Dictionary<string, object> FilterCriteria { get; set; } = new();
}

/// <summary>
/// User participation information
/// </summary>
public class UserParticipationInfoDto
{
    public bool HasUserResponse { get; set; }
    public bool CanUserParticipate { get; set; }
    public bool CanParticipate { get; set; } // Legacy property for backward compatibility
    public string? ParticipationMessage { get; set; }
    public int UserAttemptCount { get; set; }
    public int RemainingAttempts { get; set; }
    public DateTime? LastAttemptAt { get; set; }
    public DateTime? NextAttemptAllowedAt { get; set; }
    public bool IsInCoolDown { get; set; }
    public TimeSpan? CoolDownRemaining { get; set; }
    public string? CoolDownRemainingText { get; set; }
}

/// <summary>
/// Comprehensive survey statistics
/// </summary>
public class SurveyStatisticsDto
{
    public int TotalQuestions { get; set; }
    public int RequiredQuestions { get; set; }
    public int ResponseCount { get; set; }
    public int UniqueParticipantCount { get; set; }
    public int SubmittedResponseCount { get; set; }
    public int ActiveResponseCount { get; set; }
    public int CanceledResponseCount { get; set; }
    public int ExpiredResponseCount { get; set; }
    public decimal AverageCompletionPercentage { get; set; }
    public decimal AverageResponseTime { get; set; }
    public bool IsAcceptingResponses { get; set; }
    public string IsAcceptingResponsesText { get; set; } = string.Empty;
}

/// <summary>
/// Survey timing information
/// </summary>
public class SurveyTimingDto
{
    public TimeSpan? Duration { get; set; }
    public string? DurationText { get; set; }
    public TimeSpan? TimeRemaining { get; set; }
    public string? TimeRemainingText { get; set; }
    public TimeSpan? TimeElapsed { get; set; }
    public string? TimeElapsedText { get; set; }
    public bool IsExpired { get; set; }
    public bool IsScheduled { get; set; }
    public bool IsActive { get; set; }
    public DateTime? NextStateChangeAt { get; set; }
    public string? NextStateChangeText { get; set; } = string.Empty;
}

/// <summary>
/// Survey status and capabilities
/// </summary>
public class SurveyStatusDto
{
    public bool IsDraft { get; set; }
    public bool IsScheduled { get; set; }
    public bool IsActive { get; set; }
    public bool IsClosed { get; set; }
    public bool IsArchived { get; set; }
    public bool CanBeEdited { get; set; }
    public bool CanBeActivated { get; set; }
    public bool CanBeClosed { get; set; }
    public bool CanBeArchived { get; set; }
    public List<string> AvailableActions { get; set; } = new();
    public string StatusMessage { get; set; } = string.Empty;
}
