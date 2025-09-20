using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Domain.Repositories;

/// <summary>
/// Repository interface for TourMemberFeature entity operations
/// </summary>
public interface ITourMemberFeatureRepository : IRepository<TourMemberFeature, Guid>
{
    /// <summary>
    /// Gets all member features required for a specific tour
    /// </summary>
    /// <param name="tourId">Tour ID</param>
    /// <returns>Collection of tour member features</returns>
    Task<IEnumerable<TourMemberFeature>> GetByTourIdAsync(Guid tourId);

    /// <summary>
    /// Gets all tours that require a specific member feature
    /// </summary>
    /// <param name="featureId">Feature ID</param>
    /// <returns>Collection of tour member features</returns>
    Task<IEnumerable<TourMemberFeature>> GetByFeatureIdAsync(string featureId);

    /// <summary>
    /// Checks if a tour requires a specific feature
    /// </summary>
    /// <param name="tourId">Tour ID</param>
    /// <param name="featureId">Feature ID</param>
    /// <returns>True if the tour requires the feature, false otherwise</returns>
    Task<bool> TourRequiresFeatureAsync(Guid tourId, string featureId);

    /// <summary>
    /// Removes all feature requirements for a tour
    /// </summary>
    /// <param name="tourId">Tour ID</param>
    Task RemoveAllByTourIdAsync(Guid tourId);

    /// <summary>
    /// Removes all tours requiring a specific feature
    /// </summary>
    /// <param name="featureId">Feature ID</param>
    Task RemoveAllByFeatureIdAsync(string featureId);
}