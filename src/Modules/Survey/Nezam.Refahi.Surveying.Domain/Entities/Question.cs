using MCA.SharedKernel.Domain;
using Nezam.Refahi.Surveying.Domain.Enums;
using Nezam.Refahi.Surveying.Domain.ValueObjects;

namespace Nezam.Refahi.Surveying.Domain.Entities;

/// <summary>
/// Question entity representing a survey question
/// Internal entity of Survey aggregate root
/// </summary>
public sealed class Question : Entity<Guid>
{
    public Guid SurveyId { get; private set; }
    public QuestionKind Kind { get; private set; }
    public string Text { get; private set; } = null!;
    public int Order { get; private set; }
    public bool IsRequired { get; private set; }
    public RepeatPolicy RepeatPolicy { get; private set; } = null!;

    // Navigation properties
    private readonly List<QuestionOption> _options = new();
    public IReadOnlyCollection<QuestionOption> Options => _options.AsReadOnly();

    // Private constructor for EF Core
    private Question() : base() { }

    /// <summary>
    /// Creates a new question using specification
    /// </summary>
    public Question(Guid surveyId, QuestionSpecification specification)
        : base(Guid.NewGuid())
    {
        if (surveyId == Guid.Empty)
            throw new ArgumentException("Survey ID cannot be empty", nameof(surveyId));

        if (specification == null)
            throw new ArgumentNullException(nameof(specification));

        SurveyId = surveyId;
        Kind = specification.Kind;
        Text = specification.Text;
        Order = specification.Order;
        IsRequired = specification.IsRequired;
        RepeatPolicy = specification.RepeatPolicy;
    }

    /// <summary>
    /// Adds an option to the question
    /// </summary>
    public QuestionOption AddOption(string text, int order)
    {
        if (Kind == QuestionKind.Textual)
            throw new InvalidOperationException("Cannot add options to textual questions");

        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Option text cannot be empty", nameof(text));

        if (order < 0)
            throw new ArgumentException("Order cannot be negative", nameof(order));

        // Validate FixedMCQ4 has exactly 4 options
        if (Kind == QuestionKind.FixedMCQ4 && _options.Count >= 4)
            throw new InvalidOperationException("FixedMCQ4 questions can only have 4 options");

        // Validate other choice questions have at least 2 options
        if ((Kind == QuestionKind.ChoiceSingle || Kind == QuestionKind.ChoiceMulti) && _options.Count >= 25)
            throw new InvalidOperationException("Choice questions cannot have more than 25 options");

        var option = new QuestionOption(Id, text, order);
        _options.Add(option);
        return option;
    }

    /// <summary>
    /// Updates the question text
    /// </summary>
    public void UpdateText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Question text cannot be empty", nameof(text));

        Text = text.Trim();
    }

    /// <summary>
    /// Updates the question order
    /// </summary>
    public void UpdateOrder(int order)
    {
        if (order < 0)
            throw new ArgumentException("Order cannot be negative", nameof(order));

        Order = order;
    }

    /// <summary>
    /// Updates the required status
    /// </summary>
    public void UpdateRequiredStatus(bool isRequired)
    {
        IsRequired = isRequired;
    }

    /// <summary>
    /// Updates the repeat policy
    /// </summary>
    public void UpdateRepeatPolicy(RepeatPolicy repeatPolicy)
    {
        RepeatPolicy = repeatPolicy ?? throw new ArgumentNullException(nameof(repeatPolicy));
    }

    /// <summary>
    /// Gets options ordered by their order property
    /// </summary>
    public IEnumerable<QuestionOption> GetOrderedOptions()
    {
        return _options.OrderBy(o => o.Order);
    }

    /// <summary>
    /// Gets an option by ID
    /// </summary>
    public QuestionOption? GetOption(Guid optionId)
    {
        return _options.FirstOrDefault(o => o.Id == optionId);
    }

    /// <summary>
    /// Checks if the question has the minimum required options
    /// </summary>
    public bool HasValidOptions()
    {
        return Kind switch
        {
            QuestionKind.Textual => true, // No options needed
            QuestionKind.FixedMCQ4 => _options.Count == 4,
            QuestionKind.ChoiceSingle or QuestionKind.ChoiceMulti => _options.Count >= 2,
            _ => false
        };
    }

    /// <summary>
    /// Validates if selected option IDs are valid for this question
    /// </summary>
    public bool ValidateSelectedOptions(IEnumerable<Guid> selectedOptionIds)
    {
        if (Kind == QuestionKind.Textual)
            return !selectedOptionIds.Any(); // No options should be selected

        var validOptionIds = _options.Select(o => o.Id).ToHashSet();
        var selectedIds = selectedOptionIds.ToHashSet();

        // All selected options must be valid
        if (!selectedIds.IsSubsetOf(validOptionIds))
            return false;

        // Single choice questions can only have one selection
        if (Kind == QuestionKind.ChoiceSingle && selectedIds.Count > 1)
            return false;

        // Required questions must have at least one selection
        if (IsRequired && !selectedIds.Any())
            return false;

        return true;
    }

    /// <summary>
    /// Validates if a repeat index is valid for this question
    /// </summary>
    public bool ValidateRepeatIndex(int repeatIndex)
    {
        return RepeatPolicy.IsValidRepeatIndex(repeatIndex);
    }

    /// <summary>
    /// Checks if more repeats can be added for this question
    /// </summary>
    public bool CanAddMoreRepeats(int currentRepeatCount)
    {
        return RepeatPolicy.CanAddMoreRepeats(currentRepeatCount);
    }

    /// <summary>
    /// Gets the maximum allowed repeat index for this question
    /// </summary>
    public int? GetMaxRepeatIndex()
    {
        return RepeatPolicy.GetMaxRepeatIndex();
    }

    /// <summary>
    /// Checks if this question is repeatable
    /// </summary>
    public bool IsRepeatable => RepeatPolicy.Kind != RepeatPolicyKind.None;
}
