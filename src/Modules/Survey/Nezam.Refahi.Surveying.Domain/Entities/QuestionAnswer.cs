using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Surveying.Domain.Entities;

/// <summary>
/// Question answer entity representing an answer to a specific question in a response
/// This is a child entity of Response aggregate root
/// </summary>
public sealed class QuestionAnswer : Entity<Guid>
{
    public Guid ResponseId { get; private set; }
    public Guid QuestionId { get; private set; }
    public int RepeatIndex { get; private set; }
    public string? TextAnswer { get; private set; }
    public bool HasTextAnswer => !string.IsNullOrWhiteSpace(TextAnswer);

    // Navigation properties
    private readonly List<QuestionAnswerOption> _selectedOptions = new();
    public IReadOnlyCollection<QuestionAnswerOption> SelectedOptions => _selectedOptions.AsReadOnly();
    
    // EF Core navigation property
    public Response Response { get; private set; } = null!;

    // Private constructor for EF Core
    private QuestionAnswer() : base() { }

    /// <summary>
    /// Creates a new question answer
    /// </summary>
    public QuestionAnswer(Guid responseId, Guid questionId, int repeatIndex = 1)
        : base(Guid.NewGuid())
    {
        if (responseId == Guid.Empty)
            throw new ArgumentException("Response ID cannot be empty", nameof(responseId));

        if (questionId == Guid.Empty)
            throw new ArgumentException("Question ID cannot be empty", nameof(questionId));

        if (repeatIndex < 1)
            throw new ArgumentException("Repeat index must be at least 1", nameof(repeatIndex));

        ResponseId = responseId;
        QuestionId = questionId;
        RepeatIndex = repeatIndex;
    }

    /// <summary>
    /// Sets the text answer for this question
    /// </summary>
    public void SetTextAnswer(string? text)
    {
        TextAnswer = string.IsNullOrWhiteSpace(text) ? null : text.Trim();
    }

    /// <summary>
    /// Adds a selected option to this answer
    /// </summary>
    public void AddSelectedOption(Guid optionId, string optionText)
    {
        if (optionId == Guid.Empty)
            throw new ArgumentException("Option ID cannot be empty", nameof(optionId));

        if (string.IsNullOrWhiteSpace(optionText))
            throw new ArgumentException("Option text cannot be empty", nameof(optionText));

        // Check if option is already selected
        if (_selectedOptions.Any(so => so.OptionId == optionId))
            return; // Already selected

        var selectedOption = new QuestionAnswerOption(Id, optionId, optionText);
        _selectedOptions.Add(selectedOption);
    }

    /// <summary>
    /// Removes a selected option from this answer
    /// </summary>
    public void RemoveSelectedOption(Guid optionId)
    {
        var optionToRemove = _selectedOptions.FirstOrDefault(so => so.OptionId == optionId);
        if (optionToRemove != null)
        {
            _selectedOptions.Remove(optionToRemove);
        }
    }

    /// <summary>
    /// Clears all selected options
    /// </summary>
    public void ClearSelectedOptions()
    {
        _selectedOptions.Clear();
    }

    /// <summary>
    /// Gets all selected option IDs
    /// </summary>
    public IEnumerable<Guid> GetSelectedOptionIds()
    {
        return _selectedOptions.Select(so => so.OptionId);
    }

    /// <summary>
    /// Checks if this answer has any content (text or selected options)
    /// </summary>
    public bool HasAnswer()
    {
        return HasTextAnswer || _selectedOptions.Any();
    }

    /// <summary>
    /// Checks if this answer is complete for a required question
    /// </summary>
    public bool IsCompleteForRequiredQuestion()
    {
        return HasAnswer();
    }

    /// <summary>
    /// Updates the repeat index
    /// </summary>
    public void UpdateRepeatIndex(int repeatIndex)
    {
        if (repeatIndex < 1)
            throw new ArgumentException("Repeat index must be at least 1", nameof(repeatIndex));

        RepeatIndex = repeatIndex;
    }

    /// <summary>
    /// Gets a unique key for this answer (ResponseId, QuestionId, RepeatIndex)
    /// </summary>
    public string GetUniqueKey()
    {
        return $"{ResponseId}_{QuestionId}_{RepeatIndex}";
    }
}
