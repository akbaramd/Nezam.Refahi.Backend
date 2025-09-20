using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Membership.Domain.Entities;
using Nezam.Refahi.Membership.Domain.Repositories;

namespace Nezam.Refahi.Membership.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementation of role repository interface using EF Core
/// </summary>
public class RoleRepository : EfRepository<MembershipDbContext, Role, Guid>, IRoleRepository
{
    public RoleRepository(MembershipDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Role?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .FirstOrDefaultAsync(r => r.Key == key, cancellationToken:cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetActiveRolesAsync(CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Where(r => r.IsActive)
            .OrderBy(r => r.SortOrder)
            .ThenBy(r => r.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetByEmployerAsync(string employerName, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Where(r => r.EmployerName == employerName)
            .OrderBy(r => r.SortOrder)
            .ThenBy(r => r.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetByEmployerCodeAsync(string employerCode, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .Where(r => r.EmployerCode == employerCode)
            .OrderBy(r => r.SortOrder)
            .ThenBy(r => r.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Role>> SearchByTitleAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var searchTermLower = searchTerm.ToLowerInvariant();
        return await this.PrepareQuery(this._dbSet)
            .Where(r => r.Title.ToLower().Contains(searchTermLower))
            .OrderBy(r => r.SortOrder)
            .ThenBy(r => r.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .AnyAsync(r => r.Key == key, cancellationToken:cancellationToken);
    }

    public async Task<bool> ExistsByKeyAsync(string key, Guid excludeId, CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .AnyAsync(r => r.Key == key && r.Id != excludeId, cancellationToken:cancellationToken);
    }

    public async Task<IEnumerable<(Role Role, int MemberCount)>> GetRolesWithMemberCountAsync(CancellationToken cancellationToken = default)
    {
        return await this.PrepareQuery(this._dbSet)
            .GroupJoin(
                _dbContext.MemberRoles.Where(mr => mr.IsActive),
                role => role.Id,
                memberRole => memberRole.RoleId,
                (role, memberRoles) => new { Role = role, MemberCount = memberRoles.Count() })
            .Select(x => new ValueTuple<Role, int>(x.Role, x.MemberCount))
            .ToListAsync(cancellationToken);
    }

    protected override IQueryable<Role> PrepareQuery(IQueryable<Role> query)
    {
        return base.PrepareQuery(query);
    }
}