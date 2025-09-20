using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Membership.Domain.Entities;
using Nezam.Refahi.Membership.Domain.Repositories;

namespace Nezam.Refahi.Membership.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementation of member role repository interface using EF Core
/// </summary>
public class MemberRoleRepository : EfRepository<MembershipDbContext, MemberRole, Guid>, IMemberRoleRepository
{
    public MemberRoleRepository(MembershipDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<MemberRole>> GetByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Where(mr => mr.MemberId == memberId)
            .Include(mr => mr.Role)
            .OrderBy(mr => mr.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MemberRole>> GetValidRolesByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Where(mr => mr.MemberId == memberId && mr.IsActive)
            .Include(mr => mr.Role)
            .OrderBy(mr => mr.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MemberRole>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Where(mr => mr.RoleId == roleId)
            .Include(mr => mr.Member)
            .OrderBy(mr => mr.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MemberRole>> GetValidMembersByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Where(mr => mr.RoleId == roleId && mr.IsActive)
            .Include(mr => mr.Member)
            .OrderBy(mr => mr.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<MemberRole?> GetMemberRoleAsync(Guid memberId, Guid roleId, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Include(mr => mr.Role)
            .Include(mr => mr.Member)
            .FirstOrDefaultAsync(mr => mr.MemberId == memberId && mr.RoleId == roleId, cancellationToken:cancellationToken);
    }

    public async Task<IEnumerable<MemberRole>> GetExpiringRolesByMemberIdAsync(Guid memberId, DateTimeOffset cutoffDate, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Where(mr => mr.MemberId == memberId && 
                        mr.IsActive && 
                        mr.ValidTo.HasValue && 
                        mr.ValidTo <= cutoffDate)
            .Include(mr => mr.Role)
            .OrderBy(mr => mr.ValidTo)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MemberRole>> GetExpiringRolesAsync(DateTimeOffset cutoffDate, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Where(mr => mr.IsActive && 
                        mr.ValidTo.HasValue && 
                        mr.ValidTo <= cutoffDate)
            .Include(mr => mr.Role)
            .Include(mr => mr.Member)
            .OrderBy(mr => mr.ValidTo)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> MemberHasRoleAsync(Guid memberId, Guid roleId, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .AnyAsync(mr => mr.MemberId == memberId && 
                           mr.RoleId == roleId && 
                           mr.IsActive, cancellationToken:cancellationToken);
    }

    public async Task<(int ActiveAssignments, int ExpiredAssignments, int TotalAssignments)> GetRoleAssignmentStatsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var query = this.PrepareQuery(this._dbSet).Where(mr => mr.RoleId == roleId);
        
        var totalAssignments = await query.CountAsync(cancellationToken);
        var activeAssignments = await query.CountAsync(mr => mr.IsActive, cancellationToken:cancellationToken);
        var expiredAssignments = await query.CountAsync(mr => mr.ValidTo.HasValue && mr.ValidTo < DateTimeOffset.UtcNow, cancellationToken:cancellationToken);

        return (activeAssignments, expiredAssignments, totalAssignments);
    }

    public async Task DeactivateAllMemberRolesAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        var memberRoles = await this.PrepareQuery(this._dbSet)
            .Where(mr => mr.MemberId == memberId && mr.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var memberRole in memberRoles)
        {
            memberRole.Deactivate();
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateAllRoleAssignmentsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var roleAssignments = await this.PrepareQuery(this._dbSet)
            .Where(mr => mr.RoleId == roleId && mr.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var assignment in roleAssignments)
        {
            assignment.Deactivate();
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    protected override IQueryable<MemberRole> PrepareQuery(IQueryable<MemberRole> query)
    {
        query = query.Include(x => x.Member)
            .Include(x => x.Role);
        return base.PrepareQuery(query);
    }
}