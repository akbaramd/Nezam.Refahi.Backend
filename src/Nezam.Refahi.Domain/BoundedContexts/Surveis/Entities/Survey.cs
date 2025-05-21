using System.ComponentModel.DataAnnotations;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Shared.Entities;

namespace Nezam.Refahi.Domain.BoundedContexts.Surveis;

/// <summary>
/// Root aggregate for a survey created by the welfare officer.
/// Supports scheduled opening/closing and an optional per-response timer.
/// </summary>
public class Survey : BaseEntity
{
    public string Title { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    /// <summary>
    /// Determines whether the survey behaves as a quick poll (single/multi-choice only)
    /// or as a full Q&A form (mixed question types).
    /// </summary>
    public SurveyMode Mode { get; private set; }

    public SurveyStatus Status { get; private set; } = SurveyStatus.Draft;

    /// <summary>
    /// When the survey becomes visible/fillable (UTC).
    /// </summary>
    public DateTimeOffset OpensAtUtc { get; private set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// When the survey automatically expires (UTC). Null means no hard deadline.
    /// </summary>
    public DateTimeOffset? ClosesAtUtc { get; private set; }

    /// <summary>
    /// Optional timer per respondent (seconds). If set, each participant must submit before the limit.
    /// </summary>
    public int? TimeLimitSeconds { get; private set; }

    /// <summary>
    /// Identifier of the welfare officer who created the survey
    /// </summary>
    public Guid CreatorId { get; private set; }
    
    /// <summary>
    /// Reference to the User who created this survey
    /// </summary>
    public User Creator { get; private set; } = null!;

    public ICollection<SurveyQuestion> Questions { get; private set; } = new List<SurveyQuestion>();
    public ICollection<SurveyResponse> Responses { get; private set; } = new List<SurveyResponse>();
    
    // Private constructor for EF Core
    private Survey() : base() { }
    
    public Survey(string title, string? description, SurveyMode mode, User creator, 
                DateTimeOffset opensAtUtc, DateTimeOffset? closesAtUtc = null, 
                int? timeLimitSeconds = null) : base()
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
            
        Title = title;
        Description = description;
        Mode = mode;
        Status = SurveyStatus.Draft;
        OpensAtUtc = opensAtUtc;
        ClosesAtUtc = closesAtUtc;
        TimeLimitSeconds = timeLimitSeconds;
        Creator = creator ?? throw new ArgumentNullException(nameof(creator));
        CreatorId = creator.Id;
    }
    
    public void UpdateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
            
        Title = title;
        UpdateModifiedAt();
    }
    
    public void UpdateDescription(string? description)
    {
        Description = description;
        UpdateModifiedAt();
    }
    
    public void UpdateSchedule(DateTimeOffset opensAt, DateTimeOffset? closesAt)
    {
        OpensAtUtc = opensAt;
        ClosesAtUtc = closesAt;
        UpdateModifiedAt();
    }
    
    public void UpdateTimeLimit(int? timeLimitSeconds)
    {
        TimeLimitSeconds = timeLimitSeconds;
        UpdateModifiedAt();
    }
    
    public void UpdateStatus(SurveyStatus status)
    {
        Status = status;
        UpdateModifiedAt();
    }
    
    public void Publish()
    {
        if (Status == SurveyStatus.Draft || Status == SurveyStatus.Scheduled)
        {
            Status = SurveyStatus.Published;
            UpdateModifiedAt();
        }
    }
    
    public void Close()
    {
        if (Status == SurveyStatus.Published)
        {
            Status = SurveyStatus.Closed;
            UpdateModifiedAt();
        }
    }
    
    public void Archive()
    {
        if (Status == SurveyStatus.Closed)
        {
            Status = SurveyStatus.Archived;
            UpdateModifiedAt();
        }
    }

    /// <summary>
    /// Checks if the survey is currently open for submission.
    /// </summary>
    public bool IsOpen(DateTimeOffset nowUtc) =>
        Status == SurveyStatus.Published &&
        nowUtc >= OpensAtUtc &&
        (ClosesAtUtc == null || nowUtc <= ClosesAtUtc);
    }

    public enum QuestionType
    {
        SingleChoice = 0,
        MultipleChoice = 1,
        Rating = 2,
        OpenText = 3,
        FileUpload = 4
    }
