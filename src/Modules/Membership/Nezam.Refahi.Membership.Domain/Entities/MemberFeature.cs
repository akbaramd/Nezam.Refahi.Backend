using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Membership.Domain.Entities;

/// <summary>
/// Junction entity representing the many-to-many relationship between Member and Feature
/// Tracks feature assignments with validity periods and provenance
/// </summary>
public sealed class MemberFeature : Entity<Guid>
{
    public Guid MemberId { get; private set; }
    public string FeatureKey { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    // Cached feature information for performance
    public string FeatureTitle { get; private set; } = string.Empty;

    public DateTime? ValidFrom { get; private set; }
    public DateTime? ValidTo { get; private set; }
    public string? AssignedBy { get; private set; }      // Who assigned this feature
    public DateTime AssignedAt { get; private set; }    // When was it assigned
    public string? Notes { get; private set; }          // Additional context

    // Navigation properties - Only Member, no Feature relation
    public Member Member { get; private set; } = null!;
    // Feature relation removed - only store the key

    // Private constructor for EF Core
    private MemberFeature() : base() { }

    public MemberFeature(Guid memberId, string featureKey, string featureTitle,
        DateTime? validFrom = null, DateTime? validTo = null,
        string? assignedBy = null, string? notes = null)
        : base(Guid.NewGuid())
    {
        if (memberId == Guid.Empty)
            throw new ArgumentException("Member ID cannot be empty", nameof(memberId));
        if (string.IsNullOrWhiteSpace(featureKey))
            throw new ArgumentException("Feature Key cannot be empty", nameof(featureKey));
        if (string.IsNullOrWhiteSpace(featureTitle))
            throw new ArgumentException("Feature Title cannot be empty", nameof(featureTitle));

        MemberId = memberId;
        FeatureKey = featureKey.Trim();
        FeatureTitle = featureTitle.Trim();
        ValidFrom = validFrom;
        ValidTo = validTo;
        AssignedBy = assignedBy?.Trim();
        AssignedAt = DateTime.UtcNow;
        Notes = notes?.Trim();
    }

    /// <summary>
    /// Updates the feature information
    /// </summary>
    public void UpdateFeatureInformation(string featureTitle)
    {
        if (string.IsNullOrWhiteSpace(featureTitle))
            throw new ArgumentException("Feature Title cannot be empty", nameof(featureTitle));

        FeatureTitle = featureTitle.Trim();
    }

    /// <summary>
    /// Activates the feature assignment
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Deactivates the feature assignment
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Checks if the feature assignment is currently valid (active and within validity period)
    /// </summary>
    public bool IsValid()
    {
        if (!IsActive)
            return false;

        var now = DateTimeOffset.UtcNow;

        if (ValidFrom.HasValue && now < ValidFrom.Value)
            return false;

        if (ValidTo.HasValue && now > ValidTo.Value)
            return false;

        return true;
    }

    /// <summary>
    /// Checks if the feature assignment is expired
    /// </summary>
    public bool IsExpired()
    {
        return ValidTo.HasValue && DateTimeOffset.UtcNow > ValidTo.Value;
    }

    /// <summary>
    /// Updates the validity period
    /// </summary>
    public void UpdateValidityPeriod(DateTime? validFrom, DateTime? validTo)
    {
        ValidFrom = validFrom;
        ValidTo = validTo;
    }

    /// <summary>
    /// Updates the assignment metadata
    /// </summary>
    public void UpdateAssignmentMetadata(string? assignedBy, string? notes)
    {
        AssignedBy = assignedBy?.Trim();
        Notes = notes?.Trim();
    }

    public override string ToString()
    {
        return $"{MemberId}: {FeatureKey} - {FeatureTitle}";
    }
}
