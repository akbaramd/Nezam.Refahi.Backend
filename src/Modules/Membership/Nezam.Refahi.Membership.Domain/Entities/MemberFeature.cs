using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Membership.Domain.Entities;

/// <summary>
/// Junction entity representing the many-to-many relationship between Member and Feature
/// </summary>
public sealed class MemberFeature : Entity<Guid>
{
    public Guid MemberId { get; private set; }
    public string FeatureKey { get; private set; } = string.Empty;

    // Navigation properties - Only Member, no Feature relation
    public Member Member { get; private set; } = null!;

    // Private constructor for EF Core
    private MemberFeature() : base() { }

    public MemberFeature(Guid memberId, string featureKey)
        : base(Guid.NewGuid())
    {
        if (memberId == Guid.Empty)
            throw new ArgumentException("Member ID cannot be empty", nameof(memberId));
        if (string.IsNullOrWhiteSpace(featureKey))
            throw new ArgumentException("Feature Key cannot be empty", nameof(featureKey));

        MemberId = memberId;
        FeatureKey = featureKey.Trim();
    }
}
