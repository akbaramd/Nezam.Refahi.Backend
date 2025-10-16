using MCA.SharedKernel.Domain;


namespace Nezam.Refahi.Surveying.Domain.Entities;

/// <summary>
/// Question answer option entity representing a selected option in a question answer
/// </summary>
public sealed class QuestionAnswerOption : Entity<Guid>
{
    public Guid QuestionAnswerId { get; private set; }
    public Guid OptionId { get; private set; }
    public string OptionText { get; private set; } = string.Empty;

    // Private constructor for EF Core
    private QuestionAnswerOption() : base() { }

    /// <summary>
    /// Creates a new question answer option
    /// </summary>
    public QuestionAnswerOption(Guid questionAnswerId, Guid optionId, string optionText)
        : base(Guid.NewGuid())
    {
        if (questionAnswerId == Guid.Empty)
            throw new ArgumentException("Question Answer ID cannot be empty", nameof(questionAnswerId));

        if (optionId == Guid.Empty)
            throw new ArgumentException("Option ID cannot be empty", nameof(optionId));

        if (string.IsNullOrWhiteSpace(optionText))
            throw new ArgumentException("Option text cannot be empty", nameof(optionText));

        QuestionAnswerId = questionAnswerId;
        OptionId = optionId;
        OptionText = optionText.Trim();
    }
}
