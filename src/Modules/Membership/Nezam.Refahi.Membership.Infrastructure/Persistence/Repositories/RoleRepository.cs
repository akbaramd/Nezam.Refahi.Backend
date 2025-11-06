using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Membership.Domain.Entities;
using Nezam.Refahi.Membership.Domain.Repositories;
using Nezam.Refahi.Membership.Infrastructure.Persistence;

namespace Nezam.Refahi.Membership.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of IRoleRepository
/// </summary>
public class RoleRepository : EfRepository<MembershipDbContext, Role, Guid>, IRoleRepository
{
    public RoleRepository(MembershipDbContext context) : base(context)
    {
    }

    protected override IQueryable<Role> PrepareQuery(IQueryable<Role> query)
    {
        return base.PrepareQuery(query);
    }

    public async Task<Role?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return await FindOneAsync(r => r.Key == key, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetActiveRolesAsync(CancellationToken cancellationToken = default)
    {
        return (await FindAsync(r => r.IsActive, cancellationToken: cancellationToken))
            .OrderBy(r => r.SortOrder ?? int.MaxValue)
            .ThenBy(r => r.Title)
            .ToList();
    }

    public async Task<IEnumerable<Role>> GetByEmployerAsync(string employerName, CancellationToken cancellationToken = default)
    {
        return (await FindAsync(r => r.EmployerName == employerName, cancellationToken: cancellationToken))
            .OrderBy(r => r.SortOrder ?? int.MaxValue)
            .ThenBy(r => r.Title)
            .ToList();
    }

    public async Task<IEnumerable<Role>> GetByEmployerCodeAsync(string employerCode, CancellationToken cancellationToken = default)
    {
        return (await FindAsync(r => r.EmployerCode == employerCode, cancellationToken: cancellationToken))
            .OrderBy(r => r.SortOrder ?? int.MaxValue)
            .ThenBy(r => r.Title)
            .ToList();
    }

    public async Task<IEnumerable<Role>> SearchByTitleAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var searchTermLower = searchTerm.ToLowerInvariant();
        return (await FindAsync(r => r.Title.ToLower().Contains(searchTermLower), cancellationToken: cancellationToken))
            .OrderBy(r => r.SortOrder ?? int.MaxValue)
            .ThenBy(r => r.Title)
            .ToList();
    }

    public async Task<bool> ExistsByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return await ExistsAsync(r => r.Key == key, cancellationToken: cancellationToken);
    }

    public async Task<bool> ExistsByKeyAsync(string key, Guid excludeId, CancellationToken cancellationToken = default)
    {
        return await ExistsAsync(r => r.Key == key && r.Id != excludeId, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<(Role Role, int MemberCount)>> GetRolesWithMemberCountAsync(CancellationToken cancellationToken = default)
    {
        var roles = await FindAsync(_ => true, cancellationToken: cancellationToken);
        var rolesList = roles.ToList();

        // Get active member roles count per role using PrepareQuery to access DbContext
        var query = PrepareQuery(_dbSet);
        var rolesWithCounts = await query
            .GroupJoin(
                _dbContext.MemberRoles.Where(mr => mr.IsActive),
                role => role.Id,
                memberRole => memberRole.RoleId,
                (role, memberRoles) => new { Role = role, MemberCount = memberRoles.Count() })
            .Select(x => new ValueTuple<Role, int>(x.Role, x.MemberCount))
            .ToListAsync(cancellationToken);

        return rolesWithCounts;
    }
}
