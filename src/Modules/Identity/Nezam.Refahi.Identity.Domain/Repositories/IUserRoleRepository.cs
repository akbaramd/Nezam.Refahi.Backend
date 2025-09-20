using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Identity.Domain.Entities;

namespace Nezam.Refahi.Identity.Domain.Repositories;

/// <summary>
/// Repository interface for UserRole entity
/// </summary>
public interface IUserRoleRepository : IRepository<UserRole, Guid>
{
    /// <summary>
    /// Gets user roles by user ID
    /// </summary>
    Task<IEnumerable<UserRole>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active user roles by user ID
    /// </summary>
    Task<IEnumerable<UserRole>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user roles by role ID
    /// </summary>
    Task<IEnumerable<UserRole>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active user roles by role ID
    /// </summary>
    Task<IEnumerable<UserRole>> GetActiveByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific user role assignment
    /// </summary>
    Task<UserRole?> GetByUserAndRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active user role assignment
    /// </summary>
    Task<UserRole?> GetActiveByUserAndRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets expired user roles
    /// </summary>
    Task<IEnumerable<UserRole>> GetExpiredAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user roles that will expire soon
    /// </summary>
    Task<IEnumerable<UserRole>> GetExpiringSoonAsync(TimeSpan timeThreshold, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user roles assigned by a specific user
    /// </summary>
    Task<IEnumerable<UserRole>> GetAssignedByAsync(string assignedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user roles with expiration date range
    /// </summary>
    Task<IEnumerable<UserRole>> GetByExpirationRangeAsync(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of active user roles for a user
    /// </summary>
    Task<int> GetActiveCountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of active user roles for a role
    /// </summary>
    Task<int> GetActiveCountByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has a specific role
    /// </summary>
    Task<bool> UserHasRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has an active role
    /// </summary>
    Task<bool> UserHasActiveRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);


}
