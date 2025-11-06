using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Membership.Domain.Entities;

/// <summary>
/// Junction entity representing the many-to-many relationship between Member and Agency
/// </summary>
public sealed class MemberAgency : Entity<Guid>
{
    public Guid MemberId { get; private set; }
    public Guid AgencyId { get; private set; }

    // Navigation properties
    public Member Member { get; private set; } = null!;
    // Agency relation will be handled through BasicDefinitions context

    // Private constructor for EF Core
    private MemberAgency() : base() { }

    public MemberAgency(Guid memberId, Guid agencyId)
        : base(Guid.NewGuid())
    {
        if (memberId == Guid.Empty)
            throw new ArgumentException("Member ID cannot be empty", nameof(memberId));
        if (agencyId == Guid.Empty)
            throw new ArgumentException("Representative Office ID cannot be empty", nameof(agencyId));

        MemberId = memberId;
        AgencyId = agencyId;
    }
}
