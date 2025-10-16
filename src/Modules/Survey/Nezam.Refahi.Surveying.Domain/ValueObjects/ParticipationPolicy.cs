using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Surveying.Domain.ValueObjects;

/// <summary>
/// Value object representing participation policy for surveys
/// </summary>
public sealed class ParticipationPolicy : ValueObject
{
    /// <summary>
    /// Maximum number of attempts per member
    /// </summary>
    public int MaxAttemptsPerMember { get; private set; }

    /// <summary>
    /// Whether multiple submissions are allowed
    /// </summary>
    public bool AllowMultipleSubmissions { get; private set; }

    /// <summary>
    /// Cool down period in seconds between attempts
    /// </summary>
    public int? CoolDownSeconds { get; private set; }

    /// <summary>
    /// Whether back navigation is allowed during survey
    /// </summary>
    public bool AllowBackNavigation { get; private set; }

    /// <summary>
    /// Creates a participation policy
    /// </summary>
    public ParticipationPolicy(int maxAttemptsPerMember, bool allowMultipleSubmissions, int? coolDownSeconds = null, bool allowBackNavigation = true)
    {
        if (maxAttemptsPerMember <= 0)
            throw new ArgumentException("Max attempts per member must be greater than 0", nameof(maxAttemptsPerMember));

        if (coolDownSeconds.HasValue && coolDownSeconds.Value < 0)
            throw new ArgumentException("Cool down seconds cannot be negative", nameof(coolDownSeconds));

        MaxAttemptsPerMember = maxAttemptsPerMember;
        AllowMultipleSubmissions = allowMultipleSubmissions;
        CoolDownSeconds = coolDownSeconds;
        AllowBackNavigation = allowBackNavigation;
    }

    /// <summary>
    /// Checks if a new attempt is allowed based on current attempt number
    /// </summary>
    public bool IsAttemptAllowed(int currentAttemptNumber)
    {
        if (currentAttemptNumber < 0)
            return false;

        if (currentAttemptNumber > MaxAttemptsPerMember)
            return false;

        return true;
    }

    /// <summary>
    /// Checks if cool down period has passed since last attempt
    /// </summary>
    public bool IsCoolDownPassed(DateTime? lastAttemptTime)
    {
        if (!CoolDownSeconds.HasValue || !lastAttemptTime.HasValue)
            return true;

        var timeSinceLastAttempt = DateTime.UtcNow - lastAttemptTime.Value;
        return timeSinceLastAttempt.TotalSeconds >= CoolDownSeconds.Value;
    }

    private ParticipationPolicy() { }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return MaxAttemptsPerMember;
        yield return AllowMultipleSubmissions;
        yield return CoolDownSeconds ?? 0;
    }
}
