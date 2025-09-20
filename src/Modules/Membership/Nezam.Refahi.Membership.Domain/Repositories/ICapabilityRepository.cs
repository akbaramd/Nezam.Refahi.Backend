using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Membership.Domain.Entities;

namespace Nezam.Refahi.Membership.Domain.Repositories;

/// <summary>
/// Repository interface for managing Capability aggregate
/// </summary>
public interface ICapabilityRepository : IRepository<Capability,string>
{


    /// <summary>
    /// Gets a capability by ID with its claim types included
    /// </summary>
    Task<Capability?> GetByIdWithClaimTypesAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets capabilities by name pattern
    /// </summary>
    Task<IEnumerable<Capability>> GetByNameAsync(string namePattern, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active capabilities
    /// </summary>
    Task<IEnumerable<Capability>> GetActiveCapabilitiesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets capabilities that are expiring within the specified time threshold
    /// </summary>
    Task<IEnumerable<Capability>> GetExpiringCapabilitiesAsync(TimeSpan timeThreshold, CancellationToken cancellationToken = default);


    /// <summary>
    /// Checks if a capability exists by name
    /// </summary>
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a capability exists by name, excluding a specific ID
    /// </summary>
    Task<bool> ExistsByNameAsync(string name, string excludeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets paged capabilities
    /// </summary>
    Task<(IEnumerable<Capability> Capabilities, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? searchTerm = null,
        bool? isActive = null, CancellationToken cancellationToken = default);



    /// <summary>
    /// Gets count of capabilities by status
    /// </summary>
    Task<int> GetCountByStatusAsync(bool isActive, CancellationToken cancellationToken = default);

  
}