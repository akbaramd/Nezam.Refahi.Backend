using MCA.SharedKernel.Domain;
using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Membership.Domain.Entities;

namespace Nezam.Refahi.Membership.Domain.Repositories;

/// <summary>
/// Repository interface for Role aggregate root
/// </summary>
public interface IRoleRepository : IRepository<Role, Guid>
{
    /// <summary>
    /// Gets a role by its unique key
    /// </summary>
    Task<Role?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active roles ordered by sort order and then by title
    /// </summary>
    Task<IEnumerable<Role>> GetActiveRolesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets roles by employer name
    /// </summary>
    Task<IEnumerable<Role>> GetByEmployerAsync(string employerName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets roles by employer code
    /// </summary>
    Task<IEnumerable<Role>> GetByEmployerCodeAsync(string employerCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches roles by title (supports both English and Persian)
    /// </summary>
    Task<IEnumerable<Role>> SearchByTitleAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a role key already exists
    /// </summary>
    Task<bool> ExistsByKeyAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a role key already exists excluding a specific ID
    /// </summary>
    Task<bool> ExistsByKeyAsync(string key, Guid excludeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets roles with their assigned member count
    /// </summary>
    Task<IEnumerable<(Role Role, int MemberCount)>> GetRolesWithMemberCountAsync(CancellationToken cancellationToken = default);
}