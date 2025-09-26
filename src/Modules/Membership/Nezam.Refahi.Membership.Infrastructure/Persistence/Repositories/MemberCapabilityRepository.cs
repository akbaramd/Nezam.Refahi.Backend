using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Membership.Domain.Entities;
using Nezam.Refahi.Membership.Domain.Repositories;

namespace Nezam.Refahi.Membership.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementation of MemberCapability repository interface using EF Core
/// </summary>
public class MemberCapabilityRepository : EfRepository<MembershipDbContext, MemberCapability, Guid>, IMemberCapabilityRepository
{
    public MemberCapabilityRepository(MembershipDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<MemberCapability>> GetByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Where(mc => mc.MemberId == memberId)
            .OrderByDescending(mc => mc.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MemberCapability>> GetValidByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Where(mc => mc.MemberId == memberId && mc.IsValid())
            .OrderByDescending(mc => mc.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MemberCapability>> GetByCapabilityIdAsync(string capabilityId, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Where(mc => mc.CapabilityId == capabilityId)
            .OrderByDescending(mc => mc.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MemberCapability>> GetValidByCapabilityIdAsync(string capabilityId, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Where(mc => mc.CapabilityId == capabilityId && mc.IsValid())
            .OrderByDescending(mc => mc.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<MemberCapability?> GetByMemberAndCapabilityAsync(Guid memberId, string capabilityId, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Include(mc => mc.Member)
            .FirstOrDefaultAsync(mc => mc.MemberId == memberId && mc.CapabilityId == capabilityId, cancellationToken:cancellationToken);
    }

    public async Task<MemberCapability?> GetActiveByMemberAndCapabilityAsync(Guid memberId, string capabilityId, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Include(mc => mc.Member)
            .FirstOrDefaultAsync(mc => mc.MemberId == memberId &&
                               mc.CapabilityId == capabilityId &&
                               mc.IsActive, cancellationToken:cancellationToken);
    }

    public async Task<IEnumerable<MemberCapability>> GetExpiringByMemberIdAsync(Guid memberId, DateTimeOffset cutoffDate, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Where(mc => mc.MemberId == memberId &&
                        mc.IsActive &&
                        mc.ValidTo.HasValue &&
                        mc.ValidTo.Value <= cutoffDate &&
                        !mc.IsExpired())
            .OrderBy(mc => mc.ValidTo)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MemberCapability>> GetExpiringAssignmentsAsync(DateTimeOffset cutoffDate, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Include(mc => mc.Member)
            .Where(mc => mc.IsActive &&
                        mc.ValidTo.HasValue &&
                        mc.ValidTo.Value <= cutoffDate &&
                        !mc.IsExpired())
            .OrderBy(mc => mc.ValidTo)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MemberCapability>> GetByAssignedByAsync(string assignedBy, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Include(mc => mc.Member)
            .Where(mc => mc.AssignedBy == assignedBy)
            .OrderByDescending(mc => mc.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MemberCapability>> GetByAssignmentDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Include(mc => mc.Member)
            .Where(mc => mc.AssignedAt >= fromDate && mc.AssignedAt <= toDate)
            .OrderByDescending(mc => mc.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> MemberHasCapabilityAsync(Guid memberId, string capabilityId, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .AnyAsync(mc => mc.MemberId == memberId &&
                           mc.CapabilityId == capabilityId &&
                           mc.IsActive, cancellationToken:cancellationToken);
    }

    public async Task<(int TotalAssignments, int ActiveAssignments, int ExpiredAssignments)> GetCapabilityStatsAsync(string capabilityId, CancellationToken cancellationToken = default)
    {
        var query = this.PrepareQuery(this._dbSet)
            .Where(mc => mc.CapabilityId == capabilityId);

        var total = await query.CountAsync(cancellationToken);
        var active = await query.CountAsync(mc => mc.IsActive, cancellationToken:cancellationToken);
        var expired = await query.CountAsync(mc => mc.IsExpired(), cancellationToken:cancellationToken);

        return (total, active, expired);
    }

    public async Task<IEnumerable<MemberCapability>> GetExpiredAssignmentsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        return await this.PrepareQuery(this._dbSet)
            .Where(mc => mc.ValidTo.HasValue && mc.ValidTo.Value < now)
            .OrderBy(mc => mc.ValidTo)
            .ToListAsync(cancellationToken);
    }

    public async Task RemoveAllMemberCapabilitiesAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        var memberCapabilities = await this.PrepareQuery(this._dbSet)
            .Where(mc => mc.MemberId == memberId)
            .ToListAsync(cancellationToken);

        foreach (var mc in memberCapabilities)
        {
            mc.Deactivate();
        }

        await this._dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAllCapabilityAssignmentsAsync(string capabilityId, CancellationToken cancellationToken = default)
    {
        var capabilityAssignments = await this.PrepareQuery(this._dbSet)
            .Where(mc => mc.CapabilityId == capabilityId)
            .ToListAsync(cancellationToken);

        foreach (var mc in capabilityAssignments)
        {
            mc.Deactivate();
        }

        await this._dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Guid>> GetMembersWithMultipleCapabilitiesAsync(CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Where(mc => mc.IsActive)
            .GroupBy(mc => mc.MemberId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<Guid, int>> GetCapabilityCountPerMemberAsync(CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Where(mc => mc.IsActive)
            .GroupBy(mc => mc.MemberId)
            .ToDictionaryAsync(g => g.Key, g => g.Count(), cancellationToken:cancellationToken);
    }

    protected override IQueryable<MemberCapability> PrepareQuery(IQueryable<MemberCapability> query)
    {
        query = query.Include(x => x.Member);
        return base.PrepareQuery(query);
    }
}