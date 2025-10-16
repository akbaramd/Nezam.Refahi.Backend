using MCA.SharedKernel.Domain;
using Nezam.Refahi.Surveying.Domain.Enums;

namespace Nezam.Refahi.Surveying.Domain.ValueObjects;

/// <summary>
/// Value object representing the specification for creating a question
/// Encapsulates all required parameters for question creation
/// </summary>
public sealed class QuestionSpecification : ValueObject
{
    public QuestionKind Kind { get; }
    public string Text { get; }
    public int Order { get; }
    public bool IsRequired { get; }
    public RepeatPolicy RepeatPolicy { get; }

    public QuestionSpecification(
        QuestionKind kind,
        string text,
        int order,
        bool isRequired = false,
        RepeatPolicy? repeatPolicy = null)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Question text cannot be empty", nameof(text));

        if (order < 0)
            throw new ArgumentException("Order cannot be negative", nameof(order));

        Kind = kind;
        Text = text.Trim();
        Order = order;
        IsRequired = isRequired;
        RepeatPolicy = repeatPolicy ?? RepeatPolicy.None();
    }

    private QuestionSpecification() 
    {
        Kind = QuestionKind.Textual;
        Text = string.Empty;
        Order = 0;
        IsRequired = false;
        RepeatPolicy = RepeatPolicy.None();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Kind;
        yield return Text;
        yield return Order;
        yield return IsRequired;
        yield return RepeatPolicy;
    }
}
