using Nezam.Refahi.Identity.Domain.Entities;
using MCA.SharedKernel.Domain.Contracts.Repositories;

namespace Nezam.Refahi.Identity.Domain.Repositories;

/// <summary>
/// Repository interface for Role aggregate
/// </summary>
public interface IRoleRepository : IRepository<Role, Guid>
{
    /// <summary>
    /// Gets a role by name
    /// </summary>
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active roles
    /// </summary>
    Task<IEnumerable<Role>> GetActiveRolesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all system roles
    /// </summary>
    Task<IEnumerable<Role>> GetSystemRolesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all non-system roles
    /// </summary>
    Task<IEnumerable<Role>> GetNonSystemRolesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets roles by display order
    /// </summary>
    Task<IEnumerable<Role>> GetByDisplayOrderAsync(int minOrder, int maxOrder, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets roles that contain a specific claim
    /// </summary>
    Task<IEnumerable<Role>> GetByClaimAsync(string claimType, string claimValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets roles that contain any of the specified claims
    /// </summary>
    Task<IEnumerable<Role>> GetByClaimsAsync(IEnumerable<(string Type, string Value)> claims, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets roles with a specific claim type
    /// </summary>
    Task<IEnumerable<Role>> GetByClaimTypeAsync(string claimType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a role name exists
    /// </summary>
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a role name exists excluding a specific role
    /// </summary>
    Task<bool> ExistsByNameAsync(string name, Guid excludeRoleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of active roles
    /// </summary>
    Task<int> GetActiveCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of system roles
    /// </summary>
    Task<int> GetSystemRoleCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets roles created after a specific date
    /// </summary>
    Task<IEnumerable<Role>> GetCreatedAfterAsync(DateTime date, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets roles modified after a specific date
    /// </summary>
    Task<IEnumerable<Role>> GetModifiedAfterAsync(DateTime date, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets roles with user count
    /// </summary>
    Task<IEnumerable<Role>> GetWithUserCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets roles that can be deleted (no users assigned)
    /// </summary>
    Task<IEnumerable<Role>> GetDeletableRolesAsync(CancellationToken cancellationToken = default);
}
