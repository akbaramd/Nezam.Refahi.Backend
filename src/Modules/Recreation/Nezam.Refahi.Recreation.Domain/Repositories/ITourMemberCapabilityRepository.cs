using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Domain.Repositories;

/// <summary>
/// Repository interface for TourMemberCapability entity operations
/// </summary>
public interface ITourMemberCapabilityRepository : IRepository<TourMemberCapability, Guid>
{
    /// <summary>
    /// Gets all member capabilities required for a specific tour
    /// </summary>
    /// <param name="tourId">Tour ID</param>
    /// <returns>Collection of tour member capabilities</returns>
    Task<IEnumerable<TourMemberCapability>> GetByTourIdAsync(Guid tourId);

    /// <summary>
    /// Gets all tours that require a specific member capability
    /// </summary>
    /// <param name="capabilityId">Capability ID</param>
    /// <returns>Collection of tour member capabilities</returns>
    Task<IEnumerable<TourMemberCapability>> GetByCapabilityIdAsync(string capabilityId);

    /// <summary>
    /// Checks if a tour requires a specific capability
    /// </summary>
    /// <param name="tourId">Tour ID</param>
    /// <param name="capabilityId">Capability ID</param>
    /// <returns>True if the tour requires the capability, false otherwise</returns>
    Task<bool> TourRequiresCapabilityAsync(Guid tourId, string capabilityId);

    /// <summary>
    /// Removes all capability requirements for a tour
    /// </summary>
    /// <param name="tourId">Tour ID</param>
    Task RemoveAllByTourIdAsync(Guid tourId);

    /// <summary>
    /// Removes all tours requiring a specific capability
    /// </summary>
    /// <param name="capabilityId">Capability ID</param>
    Task RemoveAllByCapabilityIdAsync(string capabilityId);
}