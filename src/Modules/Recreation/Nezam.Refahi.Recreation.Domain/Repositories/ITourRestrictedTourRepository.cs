using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Domain.Repositories;

/// <summary>
/// Repository interface for TourRestrictedTour entity operations
/// </summary>
public interface ITourRestrictedTourRepository : IRepository<TourRestrictedTour, Guid>
{
    /// <summary>
    /// Gets all tours that a specific tour restricts
    /// </summary>
    /// <param name="tourId">Tour ID</param>
    /// <returns>Collection of tour restrictions</returns>
    Task<IEnumerable<TourRestrictedTour>> GetRestrictedByTourIdAsync(Guid tourId);

    /// <summary>
    /// Gets all tours that restrict a specific tour
    /// </summary>
    /// <param name="restrictedTourId">Restricted tour ID</param>
    /// <returns>Collection of tour restrictions</returns>
    Task<IEnumerable<TourRestrictedTour>> GetRestrictingTourIdAsync(Guid restrictedTourId);

    /// <summary>
    /// Checks if one tour restricts another
    /// </summary>
    /// <param name="tourId">Tour ID</param>
    /// <param name="restrictedTourId">Restricted tour ID</param>
    /// <returns>True if the tour restricts the other tour, false otherwise</returns>
    Task<bool> TourRestrictsOtherAsync(Guid tourId, Guid restrictedTourId);

    /// <summary>
    /// Removes all restrictions for a tour (both as restrictor and restricted)
    /// </summary>
    /// <param name="tourId">Tour ID</param>
    Task RemoveAllByTourIdAsync(Guid tourId);

    /// <summary>
    /// Gets mutual restrictions between two tours
    /// </summary>
    /// <param name="tourId1">First tour ID</param>
    /// <param name="tourId2">Second tour ID</param>
    /// <returns>Collection of mutual restrictions</returns>
    Task<IEnumerable<TourRestrictedTour>> GetMutualRestrictionsAsync(Guid tourId1, Guid tourId2);
}