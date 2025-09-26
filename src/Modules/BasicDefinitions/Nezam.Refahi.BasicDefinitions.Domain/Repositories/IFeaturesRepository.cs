using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.BasicDefinitions.Domain.Entities;

namespace Nezam.Refahi.BasicDefinitions.Domain.Repositories;

/// <summary>
/// Repository interface for Features entity
/// </summary>
public interface IFeaturesRepository : IRepository<Features, string>
{
    /// <summary>
    /// Gets all features by type
    /// </summary>
    Task<IEnumerable<Features>> GetByTypeAsync(string type, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active features
    /// </summary>
    Task<IEnumerable<Features>> GetActiveFeaturesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a feature exists by key
    /// </summary>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets features by multiple keys
    /// </summary>
    Task<IEnumerable<Features>> GetByKeysAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default);
}
