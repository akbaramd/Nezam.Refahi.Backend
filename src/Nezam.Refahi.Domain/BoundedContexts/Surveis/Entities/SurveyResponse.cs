namespace Nezam.Refahi.Domain.BoundedContexts.Surveis;

/// <summary>
/// A single participant’s submission of a survey.
/// </summary>
public class Response
{
    public long Id { get; set; }
    public long SurveyId { get; set; }
    public Survey Survey { get; set; } = null!;

    /// <summary>
    /// Identifier of the user who filled out the survey (null for anonymous submissions if allowed).
    /// </summary>
    public Guid? UserId { get; set; }

    public DateTime SubmittedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
}