using MCA.SharedKernel.Domain;


namespace Nezam.Refahi.Surveying.Domain.Entities;

/// <summary>
/// Question option entity representing an option for a survey question
/// </summary>
public sealed class QuestionOption : Entity<Guid>
{
    public Guid QuestionId { get; private set; }
    public string Text { get; private set; } = null!;
    public int Order { get; private set; }
    public bool IsActive { get; private set; }

    // Private constructor for EF Core
    private QuestionOption() : base() { }

    /// <summary>
    /// Creates a new question option
    /// </summary>
    public QuestionOption(Guid questionId, string text, int order)
        : base(Guid.NewGuid())
    {
        if (questionId == Guid.Empty)
            throw new ArgumentException("Question ID cannot be empty", nameof(questionId));

        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Option text cannot be empty", nameof(text));

        if (order < 0)
            throw new ArgumentException("Order cannot be negative", nameof(order));

        QuestionId = questionId;
        Text = text.Trim();
        Order = order;
        IsActive = true;
    }

    /// <summary>
    /// Updates the option text
    /// </summary>
    public void UpdateText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Option text cannot be empty", nameof(text));

        Text = text.Trim();
    }

    /// <summary>
    /// Updates the option order
    /// </summary>
    public void UpdateOrder(int order)
    {
        if (order < 0)
            throw new ArgumentException("Order cannot be negative", nameof(order));

        Order = order;
    }

    /// <summary>
    /// Activates the option
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Deactivates the option
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }
}
