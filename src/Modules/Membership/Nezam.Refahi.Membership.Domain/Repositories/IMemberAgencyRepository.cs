using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Membership.Domain.Entities;

namespace Nezam.Refahi.Membership.Domain.Repositories;

/// <summary>
/// Repository interface for MemberAgency junction entity
/// </summary>
public interface IMemberAgencyRepository : IRepository<MemberAgency, Guid>
{
    /// <summary>
    /// Gets all office access assignments for a specific member
    /// </summary>
    Task<IEnumerable<MemberAgency>> GetByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all valid (active and non-expired) office access assignments for a specific member
    /// </summary>
    Task<IEnumerable<MemberAgency>> GetValidByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all member assignments for a specific representative office
    /// </summary>
    Task<IEnumerable<MemberAgency>> GetByAgencyIdAsync(Guid AgencyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all valid member assignments for a specific representative office
    /// </summary>
    Task<IEnumerable<MemberAgency>> GetValidByAgencyIdAsync(Guid AgencyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets specific member-office access assignment
    /// </summary>
    Task<MemberAgency?> GetByMemberAndOfficeAsync(Guid memberId, Guid AgencyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active assignment for a specific member-office pair
    /// </summary>
    Task<MemberAgency?> GetActiveByMemberAndOfficeAsync(Guid memberId, Guid AgencyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets office access assignments that are expiring soon for a member
    /// </summary>
    Task<IEnumerable<MemberAgency>> GetExpiringByMemberIdAsync(Guid memberId, DateTimeOffset cutoffDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all expiring office access assignments across all members
    /// </summary>
    Task<IEnumerable<MemberAgency>> GetExpiringAssignmentsAsync(DateTimeOffset cutoffDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets office access assignments by who assigned them
    /// </summary>
    Task<IEnumerable<MemberAgency>> GetByAssignedByAsync(string assignedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets office access assignments within a date range
    /// </summary>
    Task<IEnumerable<MemberAgency>> GetByAssignmentDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets office access assignments by access level
    /// </summary>
    Task<IEnumerable<MemberAgency>> GetByAccessLevelAsync(string accessLevel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets office access assignments by access level for a specific member
    /// </summary>
    Task<IEnumerable<MemberAgency>> GetByMemberAndAccessLevelAsync(Guid memberId, string accessLevel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a member has access to a specific representative office (active)
    /// </summary>
    Task<bool> MemberHasOfficeAccessAsync(Guid memberId, Guid AgencyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a member has specific access level to a representative office
    /// </summary>
    Task<bool> MemberHasOfficeAccessLevelAsync(Guid memberId, Guid AgencyId, string accessLevel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets assignment statistics for a representative office
    /// </summary>
    Task<(int TotalAssignments, int ActiveAssignments, int ExpiredAssignments)> GetOfficeStatsAsync(Guid AgencyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets expired assignments that need cleanup
    /// </summary>
    Task<IEnumerable<MemberAgency>> GetExpiredAssignmentsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all office access assignments for a member
    /// </summary>
    Task RemoveAllMemberOfficeAccessesAsync(Guid memberId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all assignments for a specific representative office
    /// </summary>
    Task RemoveAllOfficeAssignmentsAsync(Guid AgencyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets members who have access to multiple offices
    /// </summary>
    Task<IEnumerable<Guid>> GetMembersWithMultipleOfficeAccessAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of office accesses assigned to each member
    /// </summary>
    Task<Dictionary<Guid, int>> GetOfficeAccessCountPerMemberAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets representative office IDs that a member has access to
    /// </summary>
    Task<IEnumerable<Guid>> GetAccessibleOfficeIdsAsync(Guid memberId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets member IDs who have access to a specific office
    /// </summary>
    Task<IEnumerable<Guid>> GetMemberIdsWithOfficeAccessAsync(Guid AgencyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates that a representative office ID is not empty (basic validation)
    /// Note: Full office existence validation should be done in the application service
    /// by calling the BasicDefinitions module
    /// </summary>
    bool IsValidAgencyId(Guid AgencyId);
}
