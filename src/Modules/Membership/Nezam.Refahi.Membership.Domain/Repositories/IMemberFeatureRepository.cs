using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Membership.Domain.Entities;

namespace Nezam.Refahi.Membership.Domain.Repositories;

/// <summary>
/// Repository interface for MemberFeature entity
/// </summary>
public interface IMemberFeatureRepository : IRepository<MemberFeature, Guid>
{
    /// <summary>
    /// Gets all features for a specific member
    /// </summary>
    Task<IEnumerable<MemberFeature>> GetByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all features by feature key across all members
    /// </summary>
    Task<IEnumerable<MemberFeature>> GetByFeatureKeyAsync(string featureKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all features by multiple feature keys
    /// </summary>
    Task<IEnumerable<MemberFeature>> GetByFeatureKeysAsync(IEnumerable<string> featureKeys, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all features for multiple members
    /// </summary>
    Task<IEnumerable<MemberFeature>> GetByMemberIdsAsync(IEnumerable<Guid> memberIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a member has a specific feature
    /// </summary>
    Task<bool> HasFeatureAsync(Guid memberId, string featureKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets feature keys that a member has access to
    /// </summary>
    Task<IEnumerable<string>> GetFeatureKeysByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all feature assignments for a member
    /// </summary>
    Task RemoveAllMemberFeaturesAsync(Guid memberId, CancellationToken cancellationToken = default);
}
