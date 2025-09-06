using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Identity.Infrastructure.Persistence;


namespace Nezam.Refahi.Identity.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of IRoleClaimRepository
/// </summary>
public class RoleClaimRepository : EfRepository<IdentityDbContext, RoleClaim, Guid>, IRoleClaimRepository
{
    public RoleClaimRepository(IdentityDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<RoleClaim>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RoleClaims
            .Include(rc => rc.Role)
            .Where(rc => rc.RoleId == roleId)
            .OrderBy(rc => rc.Claim.Type)
            .ThenBy(rc => rc.Claim.Value)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RoleClaim>> GetByClaimTypeAsync(string claimType, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RoleClaims
            .Include(rc => rc.Role)
            .Where(rc => rc.Claim.Type == claimType)
            .OrderBy(rc => rc.Role.Name)
            .ThenBy(rc => rc.Claim.Value)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RoleClaim>> GetByClaimAsync(string claimType, string claimValue, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RoleClaims
            .Include(rc => rc.Role)
            .Where(rc => rc.Claim.Type == claimType && rc.Claim.Value == claimValue)
            .OrderBy(rc => rc.Role.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<RoleClaim?> GetByRoleAndClaimAsync(Guid roleId, string claimType, string claimValue, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RoleClaims
            .Include(rc => rc.Role)
            .FirstOrDefaultAsync(rc => rc.RoleId == roleId && rc.Claim.Type == claimType && rc.Claim.Value == claimValue, cancellationToken);
    }

    public async Task<IEnumerable<RoleClaim>> GetByClaimTypesAsync(IEnumerable<string> claimTypes, CancellationToken cancellationToken = default)
    {
        var claimTypeList = claimTypes.ToList();
        if (!claimTypeList.Any())
            return Enumerable.Empty<RoleClaim>();

        return await _dbContext.RoleClaims
            .Include(rc => rc.Role)
            .Where(rc => claimTypeList.Contains(rc.Claim.Type))
            .OrderBy(rc => rc.Claim.Type)
            .ThenBy(rc => rc.Role.Name)
            .ThenBy(rc => rc.Claim.Value)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RoleClaim>> GetByClaimsAsync(IEnumerable<(string Type, string Value)> claims, CancellationToken cancellationToken = default)
    {
        var claimPairs = claims.ToList();
        if (!claimPairs.Any())
            return Enumerable.Empty<RoleClaim>();

        return await _dbContext.RoleClaims
            .Include(rc => rc.Role)
            .Where(rc => claimPairs.Any(claim => 
                rc.Claim.Type == claim.Type && rc.Claim.Value == claim.Value))
            .OrderBy(rc => rc.Claim.Type)
            .ThenBy(rc => rc.Role.Name)
            .ThenBy(rc => rc.Claim.Value)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCountByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RoleClaims
            .CountAsync(rc => rc.RoleId == roleId, cancellationToken);
    }

    public async Task<int> GetCountByClaimTypeAsync(string claimType, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RoleClaims
            .CountAsync(rc => rc.Claim.Type == claimType, cancellationToken);
    }

    public async Task<bool> RoleHasClaimAsync(Guid roleId, string claimType, string claimValue, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RoleClaims
            .AnyAsync(rc => rc.RoleId == roleId && rc.Claim.Type == claimType && rc.Claim.Value == claimValue, cancellationToken);
    }


}
