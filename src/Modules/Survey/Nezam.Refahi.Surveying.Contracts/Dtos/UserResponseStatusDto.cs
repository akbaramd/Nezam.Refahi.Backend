namespace Nezam.Refahi.Surveying.Contracts.Dtos;

/// <summary>
/// User's response status information
/// </summary>
public class UserResponseStatusDto
{
    public Guid ResponseId { get; set; }
    public int AttemptNumber { get; set; }
    public string AttemptStatus { get; set; } = string.Empty;
    public string AttemptStatusTextFa { get; set; } = string.Empty;
    public string ResponseStatus { get; set; } = string.Empty; // New response status
    public string ResponseStatusTextFa { get; set; } = string.Empty; // Persian text for response status
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? StartedAtLocal { get; set; }
    public DateTimeOffset? SubmittedAt { get; set; }
    public DateTimeOffset? SubmittedAtLocal { get; set; }
    public int QuestionsAnswered { get; set; }
    public int QuestionsTotal { get; set; }
    public int CompletionPercentage { get; set; }
    public bool IsActive { get; set; }
    public bool IsSubmitted { get; set; }
    public bool CanContinue { get; set; }
    public string NextActionText { get; set; } = string.Empty;
}
