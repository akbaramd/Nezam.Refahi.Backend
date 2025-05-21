using System.ComponentModel.DataAnnotations;

namespace Nezam.Refahi.Domain.BoundedContexts.Surveis;

/// <summary>
/// An individual question inside a survey.
/// </summary>
public class Question
{
    public long Id { get; set; }
    public long SurveyId { get; set; }
    public Survey Survey { get; set; } = null!;

    [Required, MaxLength(512)]
    public string Text { get; set; } = string.Empty;

    public QuestionType Type { get; set; }

    public bool IsRequired { get; set; }

    /// <summary>
    /// Zero‑based position within the survey (unique per survey).
    /// </summary>
    public int Order { get; set; }

    public ICollection<Option> Options { get; set; } = new List<Option>();
    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
}