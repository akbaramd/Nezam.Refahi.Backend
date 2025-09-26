using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Membership.Domain.Entities;
using Nezam.Refahi.Membership.Domain.Repositories;

namespace Nezam.Refahi.Membership.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementation of MemberAgency repository interface using EF Core
/// </summary>
public class MemberAgencyRepository : EfRepository<MembershipDbContext, MemberAgency, Guid>, IMemberAgencyRepository
{
    public MemberAgencyRepository(MembershipDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<MemberAgency>> GetByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Where(ma => ma.MemberId == memberId)
            .OrderByDescending(ma => ma.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MemberAgency>> GetValidByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Where(ma => ma.MemberId == memberId && ma.IsValid())
            .OrderByDescending(ma => ma.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MemberAgency>> GetByRepresentativeOfficeIdAsync(Guid representativeOfficeId, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Where(ma => ma.RepresentativeOfficeId == representativeOfficeId)
            .OrderByDescending(ma => ma.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MemberAgency>> GetValidByRepresentativeOfficeIdAsync(Guid representativeOfficeId, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Where(ma => ma.RepresentativeOfficeId == representativeOfficeId && ma.IsValid())
            .OrderByDescending(ma => ma.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<MemberAgency?> GetByMemberAndOfficeAsync(Guid memberId, Guid representativeOfficeId, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Include(ma => ma.Member)
            .FirstOrDefaultAsync(ma => ma.MemberId == memberId && ma.RepresentativeOfficeId == representativeOfficeId, cancellationToken:cancellationToken);
    }

    public async Task<MemberAgency?> GetActiveByMemberAndOfficeAsync(Guid memberId, Guid representativeOfficeId, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Include(ma => ma.Member)
            .FirstOrDefaultAsync(ma => ma.MemberId == memberId &&
                               ma.RepresentativeOfficeId == representativeOfficeId &&
                               ma.IsActive, cancellationToken:cancellationToken);
    }

    public async Task<IEnumerable<MemberAgency>> GetExpiringByMemberIdAsync(Guid memberId, DateTimeOffset cutoffDate, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Where(ma => ma.MemberId == memberId &&
                        ma.IsActive &&
                        ma.ValidTo.HasValue &&
                        ma.ValidTo.Value <= cutoffDate &&
                        !ma.IsExpired())
            .OrderBy(ma => ma.ValidTo)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MemberAgency>> GetExpiringAssignmentsAsync(DateTimeOffset cutoffDate, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Include(ma => ma.Member)
            .Where(ma => ma.IsActive &&
                        ma.ValidTo.HasValue &&
                        ma.ValidTo.Value <= cutoffDate &&
                        !ma.IsExpired())
            .OrderBy(ma => ma.ValidTo)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MemberAgency>> GetByAssignedByAsync(string assignedBy, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Include(ma => ma.Member)
            .Where(ma => ma.AssignedBy == assignedBy)
            .OrderByDescending(ma => ma.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MemberAgency>> GetByAssignmentDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Include(ma => ma.Member)
            .Where(ma => ma.AssignedAt >= fromDate && ma.AssignedAt <= toDate)
            .OrderByDescending(ma => ma.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MemberAgency>> GetByAccessLevelAsync(string accessLevel, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Include(ma => ma.Member)
            .Where(ma => ma.AccessLevel == accessLevel)
            .OrderByDescending(ma => ma.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MemberAgency>> GetByMemberAndAccessLevelAsync(Guid memberId, string accessLevel, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Where(ma => ma.MemberId == memberId && ma.AccessLevel == accessLevel)
            .OrderByDescending(ma => ma.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> MemberHasOfficeAccessAsync(Guid memberId, Guid representativeOfficeId, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .AnyAsync(ma => ma.MemberId == memberId &&
                           ma.RepresentativeOfficeId == representativeOfficeId &&
                           ma.IsActive, cancellationToken:cancellationToken);
    }

    public async Task<bool> MemberHasOfficeAccessLevelAsync(Guid memberId, Guid representativeOfficeId, string accessLevel, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .AnyAsync(ma => ma.MemberId == memberId &&
                           ma.RepresentativeOfficeId == representativeOfficeId &&
                           ma.AccessLevel == accessLevel &&
                           ma.IsActive, cancellationToken:cancellationToken);
    }

    public async Task<(int TotalAssignments, int ActiveAssignments, int ExpiredAssignments)> GetOfficeStatsAsync(Guid representativeOfficeId, CancellationToken cancellationToken = default)
    {
        var query = this.PrepareQuery(this._dbSet)
            .Where(ma => ma.RepresentativeOfficeId == representativeOfficeId);

        var total = await query.CountAsync(cancellationToken);
        var active = await query.CountAsync(ma => ma.IsActive, cancellationToken:cancellationToken);
        var expired = await query.CountAsync(ma => ma.IsExpired(), cancellationToken:cancellationToken);

        return (total, active, expired);
    }

    public async Task<IEnumerable<MemberAgency>> GetExpiredAssignmentsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        return await this.PrepareQuery(this._dbSet)
            .Where(ma => ma.ValidTo.HasValue && ma.ValidTo.Value < now)
            .OrderBy(ma => ma.ValidTo)
            .ToListAsync(cancellationToken);
    }

    public async Task RemoveAllMemberOfficeAccessesAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        var memberAgencies = await this.PrepareQuery(this._dbSet)
            .Where(ma => ma.MemberId == memberId)
            .ToListAsync(cancellationToken);

        foreach (var ma in memberAgencies)
        {
            ma.Deactivate();
        }

        await this._dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAllOfficeAssignmentsAsync(Guid representativeOfficeId, CancellationToken cancellationToken = default)
    {
        var officeAssignments = await this.PrepareQuery(this._dbSet)
            .Where(ma => ma.RepresentativeOfficeId == representativeOfficeId)
            .ToListAsync(cancellationToken);

        foreach (var ma in officeAssignments)
        {
            ma.Deactivate();
        }

        await this._dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Guid>> GetMembersWithMultipleOfficeAccessAsync(CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Where(ma => ma.IsActive)
            .GroupBy(ma => ma.MemberId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<Guid, int>> GetOfficeAccessCountPerMemberAsync(CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Where(ma => ma.IsActive)
            .GroupBy(ma => ma.MemberId)
            .ToDictionaryAsync(g => g.Key, g => g.Count(), cancellationToken:cancellationToken);
    }

    public async Task<IEnumerable<Guid>> GetAccessibleOfficeIdsAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Where(ma => ma.MemberId == memberId && ma.IsValid())
            .Select(ma => ma.RepresentativeOfficeId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Guid>> GetMemberIdsWithOfficeAccessAsync(Guid representativeOfficeId, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Where(ma => ma.RepresentativeOfficeId == representativeOfficeId && ma.IsValid())
            .Select(ma => ma.MemberId)
            .ToListAsync(cancellationToken);
    }

    public bool IsValidRepresentativeOfficeId(Guid representativeOfficeId)
    {
        return representativeOfficeId != Guid.Empty;
    }

    protected override IQueryable<MemberAgency> PrepareQuery(IQueryable<MemberAgency> query)
    {
        query = query.Include(x => x.Member);
        return base.PrepareQuery(query);
    }
}
