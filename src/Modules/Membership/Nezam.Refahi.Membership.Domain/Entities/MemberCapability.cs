using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Membership.Domain.Entities;

/// <summary>
/// Junction entity representing the many-to-many relationship between Member and Capability
/// </summary>
public sealed class MemberCapability : Entity<Guid>
{
    public Guid MemberId { get; private set; }
    public string CapabilityKey { get; private set; } = string.Empty;

    // Navigation properties - Only Member, no Capability relation
    public Member Member { get; private set; } = null!;

    // Private constructor for EF Core
    private MemberCapability() : base() { }

    public MemberCapability(Guid memberId, string capabilityKey)
        : base(Guid.NewGuid())
    {
        if (memberId == Guid.Empty)
            throw new ArgumentException("Member ID cannot be empty", nameof(memberId));
        if (string.IsNullOrWhiteSpace(capabilityKey))
            throw new ArgumentException("Capability Key cannot be empty", nameof(capabilityKey));

        MemberId = memberId;
        CapabilityKey = capabilityKey.Trim();
    }
}
