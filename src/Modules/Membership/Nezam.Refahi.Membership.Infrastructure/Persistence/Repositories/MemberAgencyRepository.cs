using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Membership.Domain.Entities;
using Nezam.Refahi.Membership.Domain.Repositories;
using Nezam.Refahi.Membership.Infrastructure.Persistence;

namespace Nezam.Refahi.Membership.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of IMemberAgencyRepository
/// </summary>
public class MemberAgencyRepository : EfRepository<MembershipDbContext, MemberAgency, Guid>, IMemberAgencyRepository
{
    public MemberAgencyRepository(MembershipDbContext context) : base(context)
    {
    }

    protected override IQueryable<MemberAgency> PrepareQuery(IQueryable<MemberAgency> query)
    {
        return query.Include(ma => ma.Member);
    }

    public async Task<IEnumerable<MemberAgency>> GetByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(ma => ma.MemberId == memberId, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<MemberAgency>> GetByAgencyIdAsync(Guid agencyId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(ma => ma.AgencyId == agencyId, cancellationToken: cancellationToken);
    }

    public async Task<MemberAgency?> GetByMemberAndAgencyAsync(Guid memberId, Guid agencyId, CancellationToken cancellationToken = default)
    {
        return await FindOneAsync(ma => ma.MemberId == memberId && ma.AgencyId == agencyId, cancellationToken: cancellationToken);
    }

    public async Task<bool> MemberHasAgencyAccessAsync(Guid memberId, Guid agencyId, CancellationToken cancellationToken = default)
    {
        return await ExistsAsync(ma => ma.MemberId == memberId && ma.AgencyId == agencyId, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<Guid>> GetAccessibleAgencyIdsAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return (await FindAsync(ma => ma.MemberId == memberId, cancellationToken: cancellationToken))
            .Select(ma => ma.AgencyId)
            .ToList();
    }

    public async Task<IEnumerable<Guid>> GetMemberIdsWithAgencyAccessAsync(Guid agencyId, CancellationToken cancellationToken = default)
    {
        return (await FindAsync(ma => ma.AgencyId == agencyId, cancellationToken: cancellationToken))
            .Select(ma => ma.MemberId)
            .ToList();
    }

    public async Task RemoveAllMemberAgencyAccessesAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        var memberAgencies = (await FindAsync(ma => ma.MemberId == memberId, cancellationToken: cancellationToken)).ToList();

        if (memberAgencies.Any())
        {
            await DeleteRangeAsync(memberAgencies, cancellationToken: cancellationToken);
        }
    }

    public async Task RemoveAllAgencyAssignmentsAsync(Guid agencyId, CancellationToken cancellationToken = default)
    {
        var agencyAssignments = (await FindAsync(ma => ma.AgencyId == agencyId, cancellationToken: cancellationToken)).ToList();

        if (agencyAssignments.Any())
        {
            await DeleteRangeAsync(agencyAssignments, cancellationToken: cancellationToken);
        }
    }

    public bool IsValidAgencyId(Guid agencyId)
    {
        return agencyId != Guid.Empty;
    }
}
