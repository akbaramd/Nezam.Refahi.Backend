using MCA.SharedKernel.Domain;
using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Membership.Domain.Entities;

namespace Nezam.Refahi.Membership.Domain.Repositories;

/// <summary>
/// Repository interface for ClaimType aggregate root
/// </summary>
public interface IFeatureRepository : IRepository<Features, string>
{
    /// <summary>
    /// Gets a claim type by its unique key
    /// </summary>
    Task<Features?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active claim types ordered by title
    /// </summary>
    Task<IEnumerable<Features>> GetActiveClaimTypesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets claim types that are required
    /// </summary>

    /// <summary>
    /// Checks if a claim type key already exists
    /// </summary>
    Task<bool> ExistsByKeyAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a claim type key already exists excluding a specific ID
    /// </summary>
    Task<bool> ExistsByKeyAsync(string key, string excludeId, CancellationToken cancellationToken = default);
}