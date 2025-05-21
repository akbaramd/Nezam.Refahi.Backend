using Nezam.Refahi.Domain.BoundedContexts.Identity.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Shared.Entities;

namespace Nezam.Refahi.Domain.BoundedContexts.Surveis;

/// <summary>
/// A single participant's submission of a survey.
/// </summary>
public class SurveyResponse : BaseEntity
{
    public Guid SurveyId { get; private set; }
    public Survey Survey { get; private set; } = null!;

    public Guid? ResponderId { get; private set; }
    public User? Responder { get; private set; }

    public DateTimeOffset StartedAtUtc { get; private set; }
    public DateTimeOffset? SubmittedAtUtc { get; private set; }

    /// <summary>
    /// Indicates whether the response exceeded the TimeLimit.
    /// </summary>
    public bool TimedOut { get; private set; }

    public ICollection<SurveyAnswer> Answers { get; private set; } = new List<SurveyAnswer>();
    
    // Private constructor for EF Core
    private SurveyResponse() : base() { }
    
    public SurveyResponse(Survey survey, User? responder = null) : base()
    {
        Survey = survey ?? throw new ArgumentNullException(nameof(survey));
        SurveyId = survey.Id;
        Responder = responder;
        ResponderId = responder?.Id;
        StartedAtUtc = DateTimeOffset.UtcNow;
    }
    
    public void Submit(bool withinTimeLimit = true)
    {
        if (SubmittedAtUtc == null)
        {
            SubmittedAtUtc = DateTimeOffset.UtcNow;
            TimedOut = !withinTimeLimit;
            UpdateModifiedAt();
        }
    }
    
    public void MarkAsTimedOut()
    {
        if (SubmittedAtUtc == null)
        {
            TimedOut = true;
            SubmittedAtUtc = DateTimeOffset.UtcNow;
            UpdateModifiedAt();
        }
    }
    
    public void AddAnswer(SurveyAnswer answer)
    {
        if (SubmittedAtUtc != null)
            throw new InvalidOperationException("Cannot add answers to an already submitted response");
            
        Answers.Add(answer);
        UpdateModifiedAt();
    }
}