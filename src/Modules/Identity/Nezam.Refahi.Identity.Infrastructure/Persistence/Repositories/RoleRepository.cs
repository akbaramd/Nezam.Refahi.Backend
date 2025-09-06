using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Identity.Infrastructure.Persistence;


namespace Nezam.Refahi.Identity.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of IRoleRepository
/// </summary>
public class RoleRepository : EfRepository<IdentityDbContext, Role, Guid>, IRoleRepository
{
    public RoleRepository(IdentityDbContext context) : base(context)
    {
    }

    public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetActiveRolesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .Where(r => r.IsActive)
            .OrderBy(r => r.DisplayOrder)
            .ThenBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetSystemRolesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .Where(r => r.IsSystemRole)
            .OrderBy(r => r.DisplayOrder)
            .ThenBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetNonSystemRolesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .Where(r => !r.IsSystemRole)
            .OrderBy(r => r.DisplayOrder)
            .ThenBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetByDisplayOrderAsync(int minOrder, int maxOrder, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .Where(r => r.DisplayOrder >= minOrder && r.DisplayOrder <= maxOrder)
            .OrderBy(r => r.DisplayOrder)
            .ThenBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetByClaimAsync(string claimType, string claimValue, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .Where(r => r.Claims.Any(c => c.Claim.Type == claimType && c.Claim.Value == claimValue))
            .OrderBy(r => r.DisplayOrder)
            .ThenBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetByClaimsAsync(IEnumerable<(string Type, string Value)> claims, CancellationToken cancellationToken = default)
    {
        var claimPairs = claims.ToList();
        if (!claimPairs.Any())
            return Enumerable.Empty<Role>();

        return await _dbContext.Roles
            .Where(r => claimPairs.Any(claim => 
                r.Claims.Any(rc => rc.Claim.Type == claim.Type && rc.Claim.Value == claim.Value)))
            .OrderBy(r => r.DisplayOrder)
            .ThenBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetByClaimTypeAsync(string claimType, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .Where(r => r.Claims.Any(c => c.Claim.Type == claimType))
            .OrderBy(r => r.DisplayOrder)
            .ThenBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .AnyAsync(r => r.Name == name, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, Guid excludeRoleId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .AnyAsync(r => r.Name == name && r.Id != excludeRoleId, cancellationToken);
    }

    public async Task<int> GetActiveCountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .CountAsync(r => r.IsActive, cancellationToken);
    }

    public async Task<int> GetSystemRoleCountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .CountAsync(r => r.IsSystemRole, cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetCreatedAfterAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .Where(r => r.CreatedAt > date)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetModifiedAfterAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .Where(r => r.ModifiedAt > date)
            .OrderBy(r => r.ModifiedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetWithUserCountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .Include(r => r.UserRoles)
            .OrderBy(r => r.DisplayOrder)
            .ThenBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetDeletableRolesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .Where(r => !r.IsSystemRole && !r.UserRoles.Any(ur => ur.IsActive))
            .OrderBy(r => r.DisplayOrder)
            .ThenBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }
}
