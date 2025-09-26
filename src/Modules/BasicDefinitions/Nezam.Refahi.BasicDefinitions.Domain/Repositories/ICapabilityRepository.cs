using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.BasicDefinitions.Domain.Entities;

namespace Nezam.Refahi.BasicDefinitions.Domain.Repositories;

/// <summary>
/// Repository interface for Capability entity
/// </summary>
public interface ICapabilityRepository : IRepository<Capability, string>
{
    /// <summary>
    /// Gets all active capabilities
    /// </summary>
    Task<IEnumerable<Capability>> GetActiveCapabilitiesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets capabilities by name
    /// </summary>
    Task<IEnumerable<Capability>> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a capability exists by key
    /// </summary>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets capabilities by multiple keys
    /// </summary>
    Task<IEnumerable<Capability>> GetByKeysAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets capabilities that are valid at a specific date
    /// </summary>
    Task<IEnumerable<Capability>> GetValidCapabilitiesAsync(DateTime date, CancellationToken cancellationToken = default);
}
