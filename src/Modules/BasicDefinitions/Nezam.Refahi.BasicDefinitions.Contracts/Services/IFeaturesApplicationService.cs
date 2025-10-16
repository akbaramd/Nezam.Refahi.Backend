using Nezam.Refahi.BasicDefinitions.Contracts.DTOs;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.BasicDefinitions.Contracts.Services;

/// <summary>
/// Application service interface for Features operations
/// </summary>
public interface IFeaturesApplicationService
{
    /// <summary>
    /// Get feature by ID
    /// </summary>
    Task<ApplicationResult<FeaturesDto>> GetFeatureByIdAsync(string featureId);

    /// <summary>
    /// Get features by type
    /// </summary>
    Task<ApplicationResult<IEnumerable<FeaturesDto>>> GetFeaturesByTypeAsync(string type);

    /// <summary>
    /// Get all active features
    /// </summary>
    Task<ApplicationResult<IEnumerable<FeaturesDto>>> GetActiveFeaturesAsync();

    /// <summary>
    /// Get all features
    /// </summary>
    Task<ApplicationResult<IEnumerable<FeaturesDto>>> GetAllFeaturesAsync();

    /// <summary>
    /// Get features by multiple keys
    /// </summary>
    Task<ApplicationResult<IEnumerable<FeaturesDto>>> GetFeaturesByKeysAsync(IEnumerable<string> keys);

    /// <summary>
    /// Create a new feature
    /// </summary>
    Task<ApplicationResult<FeaturesDto>> CreateFeatureAsync(FeaturesDto featureDto);

    /// <summary>
    /// Update an existing feature
    /// </summary>
    Task<ApplicationResult<FeaturesDto>> UpdateFeatureAsync(FeaturesDto featureDto);

    /// <summary>
    /// Delete a feature
    /// </summary>
    Task<ApplicationResult<bool>> DeleteFeatureAsync(string featureId);
}
