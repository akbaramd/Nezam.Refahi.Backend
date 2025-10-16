using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Queries;

/// <summary>
/// Query to get user's participation status in a specific survey
/// </summary>
public record GetParticipationStatusQuery(Guid SurveyId, Guid MemberId) : IRequest<ApplicationResult<ParticipationStatusResponse>>;

/// <summary>
/// Response containing user's participation status
/// </summary>
public class ParticipationStatusResponse
{
    public Guid SurveyId { get; set; }
    public Guid MemberId { get; set; }
    
    // Eligibility
    public bool IsEligible { get; set; }
    public string? EligibilityReason { get; set; }
    
    // Attempts
    public int TotalAttempts { get; set; }
    public int MaxAllowedAttempts { get; set; }
    public bool CanStartNewAttempt { get; set; }
    
    // Current attempt
    public Guid? CurrentResponseId { get; set; }
    public string? CurrentAttemptStatus { get; set; } // "InProgress", "Submitted", "Abandoned"
    public DateTime? CurrentAttemptStartedAt { get; set; }
    public DateTime? CurrentAttemptSubmittedAt { get; set; }
    
    // Cooldown
    public bool IsInCooldown { get; set; }
    public DateTime? CooldownEndsAt { get; set; }
    public TimeSpan? RemainingCooldown { get; set; }
    
    // Multiple submissions
    public bool AllowMultipleSubmissions { get; set; }
    public List<AttemptSummary> PreviousAttempts { get; set; } = new();
}

/// <summary>
/// Summary of a previous attempt
/// </summary>
public class AttemptSummary
{
    public Guid ResponseId { get; set; }
    public int AttemptNumber { get; set; }
    public string Status { get; set; } = string.Empty; // "Submitted", "Abandoned"
    public DateTime? StartedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public int AnsweredQuestions { get; set; }
    public int TotalQuestions { get; set; }
    public double CompletionPercentage { get; set; }
}
