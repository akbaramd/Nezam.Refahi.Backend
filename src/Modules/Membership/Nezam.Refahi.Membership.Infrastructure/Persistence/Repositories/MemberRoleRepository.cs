using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Membership.Domain.Entities;
using Nezam.Refahi.Membership.Domain.Repositories;
using Nezam.Refahi.Membership.Infrastructure.Persistence;

namespace Nezam.Refahi.Membership.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of IMemberRoleRepository
/// </summary>
public class MemberRoleRepository : EfRepository<MembershipDbContext, MemberRole, Guid>, IMemberRoleRepository
{
    public MemberRoleRepository(MembershipDbContext context) : base(context)
    {
    }

    protected override IQueryable<MemberRole> PrepareQuery(IQueryable<MemberRole> query)
    {
        return query
            .Include(mr => mr.Member)
            .Include(mr => mr.Role);
    }

    public async Task<IEnumerable<MemberRole>> GetByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return (await FindAsync(mr => mr.MemberId == memberId, cancellationToken: cancellationToken))
            .OrderBy(mr => mr.AssignedAt)
            .ToList();
    }

    public async Task<IEnumerable<MemberRole>> GetValidRolesByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        var memberRoles = (await FindAsync(mr => mr.MemberId == memberId && mr.IsActive, cancellationToken: cancellationToken))
            .Where(mr => mr.IsValid())
            .OrderBy(mr => mr.AssignedAt)
            .ToList();

        return memberRoles;
    }

    public async Task<IEnumerable<MemberRole>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return (await FindAsync(mr => mr.RoleId == roleId, cancellationToken: cancellationToken))
            .OrderBy(mr => mr.AssignedAt)
            .ToList();
    }

    public async Task<IEnumerable<MemberRole>> GetValidMembersByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var memberRoles = (await FindAsync(mr => mr.RoleId == roleId && mr.IsActive, cancellationToken: cancellationToken))
            .Where(mr => mr.IsValid())
            .OrderBy(mr => mr.AssignedAt)
            .ToList();

        return memberRoles;
    }

    public async Task<MemberRole?> GetMemberRoleAsync(Guid memberId, Guid roleId, CancellationToken cancellationToken = default)
    {
        return await FindOneAsync(mr => mr.MemberId == memberId && mr.RoleId == roleId, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<MemberRole>> GetExpiringRolesByMemberIdAsync(Guid memberId, DateTimeOffset cutoffDate, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        return (await FindAsync(mr => mr.MemberId == memberId && 
                                     mr.IsActive && 
                                     mr.ValidTo.HasValue && 
                                     mr.ValidTo.Value <= cutoffDate &&
                                     mr.ValidTo.Value > now, 
                                     cancellationToken: cancellationToken))
            .OrderBy(mr => mr.ValidTo)
            .ToList();
    }

    public async Task<IEnumerable<MemberRole>> GetExpiringRolesAsync(DateTimeOffset cutoffDate, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        return (await FindAsync(mr => mr.IsActive && 
                                     mr.ValidTo.HasValue && 
                                     mr.ValidTo.Value <= cutoffDate &&
                                     mr.ValidTo.Value > now, 
                                     cancellationToken: cancellationToken))
            .OrderBy(mr => mr.ValidTo)
            .ToList();
    }

    public async Task<bool> MemberHasRoleAsync(Guid memberId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var memberRole = await FindOneAsync(mr => mr.MemberId == memberId && mr.RoleId == roleId, cancellationToken: cancellationToken);
        return memberRole != null && memberRole.IsActive && memberRole.IsValid();
    }

    public async Task<(int ActiveAssignments, int ExpiredAssignments, int TotalAssignments)> GetRoleAssignmentStatsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var allAssignments = (await FindAsync(mr => mr.RoleId == roleId, cancellationToken: cancellationToken)).ToList();
        var totalAssignments = allAssignments.Count;
        var activeAssignments = allAssignments.Count(mr => mr.IsActive && mr.IsValid());
        var now = DateTimeOffset.UtcNow;
        var expiredAssignments = allAssignments.Count(mr => mr.ValidTo.HasValue && mr.ValidTo.Value < now);

        return (activeAssignments, expiredAssignments, totalAssignments);
    }

    public async Task DeactivateAllMemberRolesAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        var memberRoles = (await FindAsync(mr => mr.MemberId == memberId && mr.IsActive, cancellationToken: cancellationToken)).ToList();

        foreach (var memberRole in memberRoles)
        {
            memberRole.Deactivate();
        }

        await SaveAsync(cancellationToken);
    }

    public async Task DeactivateAllRoleAssignmentsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var roleAssignments = (await FindAsync(mr => mr.RoleId == roleId && mr.IsActive, cancellationToken: cancellationToken)).ToList();

        foreach (var assignment in roleAssignments)
        {
            assignment.Deactivate();
        }

        await SaveAsync(cancellationToken);
    }
}
