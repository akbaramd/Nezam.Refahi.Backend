using MCA.SharedKernel.Domain.Contracts;
using Nezam.Refahi.Identity.Domain.Entities;

namespace Nezam.Refahi.Identity.Domain.Repositories;

/// <summary>
/// Repository interface for RoleClaim entity
/// </summary>
public interface IRoleClaimRepository : IRepository<RoleClaim, Guid>
{
    /// <summary>
    /// Gets role claims by role ID
    /// </summary>
    Task<IEnumerable<RoleClaim>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets role claims by claim type
    /// </summary>
    Task<IEnumerable<RoleClaim>> GetByClaimTypeAsync(string claimType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets role claims by claim type and value
    /// </summary>
    Task<IEnumerable<RoleClaim>> GetByClaimAsync(string claimType, string claimValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific role claim
    /// </summary>
    Task<RoleClaim?> GetByRoleAndClaimAsync(Guid roleId, string claimType, string claimValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets role claims by multiple claim types
    /// </summary>
    Task<IEnumerable<RoleClaim>> GetByClaimTypesAsync(IEnumerable<string> claimTypes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets role claims by multiple claims
    /// </summary>
    Task<IEnumerable<RoleClaim>> GetByClaimsAsync(IEnumerable<(string Type, string Value)> claims, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of role claims for a role
    /// </summary>
    Task<int> GetCountByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of role claims by claim type
    /// </summary>
    Task<int> GetCountByClaimTypeAsync(string claimType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a role has a specific claim
    /// </summary>
    Task<bool> RoleHasClaimAsync(Guid roleId, string claimType, string claimValue, CancellationToken cancellationToken = default);

    
}
