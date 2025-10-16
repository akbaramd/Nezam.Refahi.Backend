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
    /// Gets all valid (active and within validity period) features for a specific member
    /// </summary>
    Task<IEnumerable<MemberFeature>> GetValidFeaturesByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default);

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
    /// Gets expiring features for a specific member
    /// </summary>
    Task<IEnumerable<MemberFeature>> GetExpiringFeaturesByMemberIdAsync(Guid memberId, TimeSpan timeThreshold, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all expiring features across all members
    /// </summary>
    Task<IEnumerable<MemberFeature>> GetExpiringFeaturesAsync(TimeSpan timeThreshold, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets features assigned by a specific user
    /// </summary>
    Task<IEnumerable<MemberFeature>> GetByAssignedByAsync(string assignedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets features within a specific date range
    /// </summary>
    Task<IEnumerable<MemberFeature>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default);
}
