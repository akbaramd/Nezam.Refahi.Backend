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
    /// Gets all member assignments for a specific capability
    /// </summary>
    Task<IEnumerable<MemberCapability>> GetByCapabilityIdAsync(string capabilityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets specific member-capability assignment
    /// </summary>
    Task<MemberCapability?> GetByMemberAndCapabilityAsync(Guid memberId, string capabilityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a member has a specific capability assignment
    /// </summary>
    Task<bool> MemberHasCapabilityAsync(Guid memberId, string capabilityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets capability keys that a member has access to
    /// </summary>
    Task<IEnumerable<string>> GetCapabilityKeysByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets member IDs who have access to a specific capability
    /// </summary>
    Task<IEnumerable<Guid>> GetMemberIdsWithCapabilityAsync(string capabilityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all capability assignments for a member
    /// </summary>
    Task RemoveAllMemberCapabilitiesAsync(Guid memberId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all assignments of a specific capability
    /// </summary>
    Task RemoveAllCapabilityAssignmentsAsync(string capabilityId, CancellationToken cancellationToken = default);
}
