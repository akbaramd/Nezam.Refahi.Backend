using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Domain.Repositories;

/// <summary>
/// Repository interface for tour capacity management
/// </summary>
public interface ITourCapacityRepository : IRepository<TourCapacity, Guid>
{
    /// <summary>
    /// Gets all capacities for a specific tour
    /// </summary>
    Task<IEnumerable<TourCapacity>> GetByTourIdAsync(Guid tourId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active capacities for a specific tour
    /// </summary>
    Task<IEnumerable<TourCapacity>> GetActiveBytourIdAsync(Guid tourId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the effective capacity for a tour at a specific date
    /// </summary>
    Task<TourCapacity?> GetEffectiveCapacityAsync(Guid tourId, DateTime date, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets capacities that are open for registration at a specific date
    /// </summary>
    Task<IEnumerable<TourCapacity>> GetOpenForRegistrationAsync(Guid tourId, DateTime date, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets capacity by ID with tour information
    /// </summary>
    Task<TourCapacity?> GetWithTourAsync(Guid capacityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a capacity exists and is active
    /// </summary>
    Task<bool> IsActiveCapacityAsync(Guid capacityId, CancellationToken cancellationToken = default);

   
}
