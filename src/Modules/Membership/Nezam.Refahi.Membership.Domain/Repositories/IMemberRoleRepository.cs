using MCA.SharedKernel.Domain;
using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Membership.Domain.Entities;

namespace Nezam.Refahi.Membership.Domain.Repositories;

/// <summary>
/// Repository interface for MemberRole entity
/// </summary>
public interface IMemberRoleRepository : IRepository<MemberRole, Guid>
{
    /// <summary>
    /// Gets all roles for a specific member
    /// </summary>
    Task<IEnumerable<MemberRole>> GetByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all valid (active and within validity period) roles for a specific member
    /// </summary>
    Task<IEnumerable<MemberRole>> GetValidRolesByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all members with a specific role
    /// </summary>
    Task<IEnumerable<MemberRole>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all valid members with a specific role
    /// </summary>
    Task<IEnumerable<MemberRole>> GetValidMembersByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific member role assignment
    /// </summary>
    Task<MemberRole?> GetMemberRoleAsync(Guid memberId, Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets roles that are expiring soon for a member
    /// </summary>
    Task<IEnumerable<MemberRole>> GetExpiringRolesByMemberIdAsync(Guid memberId, DateTimeOffset cutoffDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all expiring roles across all members
    /// </summary>
    Task<IEnumerable<MemberRole>> GetExpiringRolesAsync(DateTimeOffset cutoffDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a member has a specific role (active and valid)
    /// </summary>
    Task<bool> MemberHasRoleAsync(Guid memberId, Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets role assignment statistics
    /// </summary>
    Task<(int ActiveAssignments, int ExpiredAssignments, int TotalAssignments)> GetRoleAssignmentStatsAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deactivates all role assignments for a member
    /// </summary>
    Task DeactivateAllMemberRolesAsync(Guid memberId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deactivates all assignments for a specific role
    /// </summary>
    Task DeactivateAllRoleAssignmentsAsync(Guid roleId, CancellationToken cancellationToken = default);
}