using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Surveying.Domain.ValueObjects;

/// <summary>
/// Represents the repeat policy for a question
/// Immutable value object following DDD principles
/// </summary>
public sealed class RepeatPolicy : ValueObject
{
    public RepeatPolicyKind Kind { get; }
    public int? MaxRepeats { get; }

    private RepeatPolicy() 
    {
        Kind = RepeatPolicyKind.None;
        MaxRepeats = null;
    } // For EF Core

    private RepeatPolicy(RepeatPolicyKind kind, int? maxRepeats = null)
    {
        Kind = kind;
        MaxRepeats = maxRepeats;
    }

    /// <summary>
    /// Creates a repeat policy that doesn't allow repeats (single answer only)
    /// </summary>
    public static RepeatPolicy None() => new(RepeatPolicyKind.None);

    /// <summary>
    /// Creates a bounded repeat policy with a maximum number of repeats
    /// </summary>
    public static RepeatPolicy Bounded(int maxRepeats)
    {
        if (maxRepeats < 1)
            throw new ArgumentException("MaxRepeats must be at least 1", nameof(maxRepeats));
        
        return new(RepeatPolicyKind.Bounded, maxRepeats);
    }

    /// <summary>
    /// Creates an unbounded repeat policy (unlimited repeats)
    /// </summary>
    public static RepeatPolicy Unbounded() => new(RepeatPolicyKind.Unbounded);

    /// <summary>
    /// Validates if a repeat index is valid for this policy
    /// </summary>
    public bool IsValidRepeatIndex(int repeatIndex)
    {
        if (repeatIndex < 1) return false;

        return Kind switch
        {
            RepeatPolicyKind.None => repeatIndex == 1,
            RepeatPolicyKind.Bounded => repeatIndex <= MaxRepeats!.Value,
            RepeatPolicyKind.Unbounded => true,
            _ => false
        };
    }

    /// <summary>
    /// Gets the maximum allowed repeat index for this policy
    /// </summary>
    public int? GetMaxRepeatIndex()
    {
        return Kind switch
        {
            RepeatPolicyKind.None => 1,
            RepeatPolicyKind.Bounded => MaxRepeats,
            RepeatPolicyKind.Unbounded => null, // Unlimited
            _ => null
        };
    }

    /// <summary>
    /// Checks if more repeats can be added
    /// </summary>
    public bool CanAddMoreRepeats(int currentRepeatCount)
    {
        return Kind switch
        {
            RepeatPolicyKind.None => currentRepeatCount == 0,
            RepeatPolicyKind.Bounded => currentRepeatCount < MaxRepeats!.Value,
            RepeatPolicyKind.Unbounded => true,
            _ => false
        };
    }

    public override string ToString()
    {
        return Kind switch
        {
            RepeatPolicyKind.None => "None (Single)",
            RepeatPolicyKind.Bounded => $"Bounded (Max: {MaxRepeats})",
            RepeatPolicyKind.Unbounded => "Unbounded (Unlimited)",
            _ => "Unknown"
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Kind;
        yield return MaxRepeats ?? 0;
    }
}

/// <summary>
/// Represents the kind of repeat policy
/// </summary>
public enum RepeatPolicyKind
{
    /// <summary>
    /// Question can only be answered once (no repeats)
    /// </summary>
    None = 0,

    /// <summary>
    /// Question can be repeated a bounded number of times
    /// </summary>
    Bounded = 1,

    /// <summary>
    /// Question can be repeated unlimited times
    /// </summary>
    Unbounded = 2
}
