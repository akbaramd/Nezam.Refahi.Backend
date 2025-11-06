using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Membership.Domain.Entities;
using Nezam.Refahi.Membership.Domain.Repositories;
using Nezam.Refahi.Membership.Infrastructure.Persistence;

namespace Nezam.Refahi.Membership.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of IMemberCapabilityRepository
/// </summary>
public class MemberCapabilityRepository : EfRepository<MembershipDbContext, MemberCapability, Guid>, IMemberCapabilityRepository
{
    public MemberCapabilityRepository(MembershipDbContext context) : base(context)
    {
    }

    protected override IQueryable<MemberCapability> PrepareQuery(IQueryable<MemberCapability> query)
    {
        return query.Include(mc => mc.Member);
    }

    public async Task<IEnumerable<MemberCapability>> GetByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(mc => mc.MemberId == memberId, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<MemberCapability>> GetByCapabilityIdAsync(string capabilityId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(mc => mc.CapabilityKey == capabilityId, cancellationToken: cancellationToken);
    }

    public async Task<MemberCapability?> GetByMemberAndCapabilityAsync(Guid memberId, string capabilityId, CancellationToken cancellationToken = default)
    {
        return await FindOneAsync(mc => mc.MemberId == memberId && mc.CapabilityKey == capabilityId, cancellationToken: cancellationToken);
    }

    public async Task<bool> MemberHasCapabilityAsync(Guid memberId, string capabilityId, CancellationToken cancellationToken = default)
    {
        return await ExistsAsync(mc => mc.MemberId == memberId && mc.CapabilityKey == capabilityId, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<string>> GetCapabilityKeysByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return (await FindAsync(mc => mc.MemberId == memberId, cancellationToken: cancellationToken))
            .Select(mc => mc.CapabilityKey)
            .ToList();
    }

    public async Task<IEnumerable<Guid>> GetMemberIdsWithCapabilityAsync(string capabilityId, CancellationToken cancellationToken = default)
    {
        return (await FindAsync(mc => mc.CapabilityKey == capabilityId, cancellationToken: cancellationToken))
            .Select(mc => mc.MemberId)
            .ToList();
    }

    public async Task RemoveAllMemberCapabilitiesAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        var memberCapabilities = (await FindAsync(mc => mc.MemberId == memberId, cancellationToken: cancellationToken)).ToList();

        if (memberCapabilities.Any())
        {
            await DeleteRangeAsync(memberCapabilities, cancellationToken: cancellationToken);
        }
    }

    public async Task RemoveAllCapabilityAssignmentsAsync(string capabilityId, CancellationToken cancellationToken = default)
    {
        var capabilityAssignments = (await FindAsync(mc => mc.CapabilityKey == capabilityId, cancellationToken: cancellationToken)).ToList();

        if (capabilityAssignments.Any())
        {
            await DeleteRangeAsync(capabilityAssignments, cancellationToken: cancellationToken);
        }
    }
}
