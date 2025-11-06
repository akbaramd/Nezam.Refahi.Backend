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
    /// Gets all member assignments for a specific representative office
    /// </summary>
    Task<IEnumerable<MemberAgency>> GetByAgencyIdAsync(Guid agencyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets specific member-office access assignment
    /// </summary>
    Task<MemberAgency?> GetByMemberAndAgencyAsync(Guid memberId, Guid agencyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a member has access to a specific representative office
    /// </summary>
    Task<bool> MemberHasAgencyAccessAsync(Guid memberId, Guid agencyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets representative office IDs that a member has access to
    /// </summary>
    Task<IEnumerable<Guid>> GetAccessibleAgencyIdsAsync(Guid memberId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets member IDs who have access to a specific office
    /// </summary>
    Task<IEnumerable<Guid>> GetMemberIdsWithAgencyAccessAsync(Guid agencyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all office access assignments for a member
    /// </summary>
    Task RemoveAllMemberAgencyAccessesAsync(Guid memberId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all assignments for a specific representative office
    /// </summary>
    Task RemoveAllAgencyAssignmentsAsync(Guid agencyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates that a representative office ID is not empty (basic validation)
    /// Note: Full office existence validation should be done in the application service
    /// by calling the BasicDefinitions module
    /// </summary>
    bool IsValidAgencyId(Guid agencyId);
}
