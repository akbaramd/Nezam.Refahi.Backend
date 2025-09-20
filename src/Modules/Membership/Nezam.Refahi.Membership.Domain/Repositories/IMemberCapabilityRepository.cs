using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Membership.Domain.Entities;

namespace Nezam.Refahi.Membership.Domain.Repositories;

/// <summary>
/// Repository interface for MemberCapability junction entity
/// </summary>
public interface IMemberCapabilityRepository : IRepository<MemberCapability, Guid>
{
    /// <summary>
    /// Gets all capability assignments for a specific member
    /// </summary>
    Task<IEnumerable<MemberCapability>> GetByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all valid (active and non-expired) capability assignments for a specific member
    /// </summary>
    Task<IEnumerable<MemberCapability>> GetValidByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all member assignments for a specific capability
    /// </summary>
    Task<IEnumerable<MemberCapability>> GetByCapabilityIdAsync(string capabilityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all valid member assignments for a specific capability
    /// </summary>
    Task<IEnumerable<MemberCapability>> GetValidByCapabilityIdAsync(string capabilityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets specific member-capability assignment
    /// </summary>
    Task<MemberCapability?> GetByMemberAndCapabilityAsync(Guid memberId, string capabilityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active assignment for a specific member-capability pair
    /// </summary>
    Task<MemberCapability?> GetActiveByMemberAndCapabilityAsync(Guid memberId, string capabilityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets capability assignments that are expiring soon for a member
    /// </summary>
    Task<IEnumerable<MemberCapability>> GetExpiringByMemberIdAsync(Guid memberId, DateTimeOffset cutoffDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all expiring capability assignments across all members
    /// </summary>
    Task<IEnumerable<MemberCapability>> GetExpiringAssignmentsAsync(DateTimeOffset cutoffDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets capability assignments by who assigned them
    /// </summary>
    Task<IEnumerable<MemberCapability>> GetByAssignedByAsync(string assignedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets capability assignments within a date range
    /// </summary>
    Task<IEnumerable<MemberCapability>> GetByAssignmentDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a member has a specific capability assignment (active)
    /// </summary>
    Task<bool> MemberHasCapabilityAsync(Guid memberId, string capabilityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets assignment statistics for a capability
    /// </summary>
    Task<(int TotalAssignments, int ActiveAssignments, int ExpiredAssignments)> GetCapabilityStatsAsync(string capabilityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets expired assignments that need cleanup
    /// </summary>
    Task<IEnumerable<MemberCapability>> GetExpiredAssignmentsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all capability assignments for a member
    /// </summary>
    Task RemoveAllMemberCapabilitiesAsync(Guid memberId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all assignments of a specific capability
    /// </summary>
    Task RemoveAllCapabilityAssignmentsAsync(string capabilityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets members who have multiple capabilities assigned
    /// </summary>
    Task<IEnumerable<Guid>> GetMembersWithMultipleCapabilitiesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of capabilities assigned to each member
    /// </summary>
    Task<Dictionary<Guid, int>> GetCapabilityCountPerMemberAsync(CancellationToken cancellationToken = default);
}