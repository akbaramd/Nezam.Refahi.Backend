namespace Nezam.Refahi.Domain.BoundedContexts.Surveis;

/// <summary>
/// An answer to a single question within a response.
/// </summary>
public class SurveyResponseAnswer
{
    public long Id { get; set; }
    public long ResponseId { get; set; }
    public SurveyResponse SurveyResponse { get; set; } = null!;

    public long QuestionId { get; set; }
    public SurveyQuestion SurveyQuestion { get; set; } = null!;

    /// <summary>
    /// Selected option (for choice questions); null for open‑text answers.
    /// </summary>
    public long? OptionId { get; set; }
    public Option? Option { get; set; }

    /// <summary>
    /// Free‑text answer (for OpenText questions).
    /// </summary>
    public string? TextAnswer { get; set; }
}