using Nezam.Refahi.BasicDefinitions.Domain.Entities;

namespace Nezam.Refahi.BasicDefinitions.Contracts.Services;

/// <summary>
/// Service interface for caching basic definitions (Capabilities, Features, and Agencyies)
/// Provides fast access to frequently used data without database hits
/// </summary>
public interface IBasicDefinitionsCacheService
{
    /// <summary>
    /// Gets all capabilities from cache
    /// </summary>
    /// <returns>Collection of capabilities</returns>
    Task<IEnumerable<Capability>> GetCapabilitiesAsync();

    /// <summary>
    /// Gets a specific capability by ID from cache
    /// </summary>
    /// <param name="capabilityId">The capability ID</param>
    /// <returns>Capability if found, null otherwise</returns>
    Task<Capability?> GetCapabilityAsync(string capabilityId);

    /// <summary>
    /// Gets all features from cache
    /// </summary>
    /// <returns>Collection of features</returns>
    Task<IEnumerable<Features>> GetFeaturesAsync();

    /// <summary>
    /// Gets a specific feature by ID from cache
    /// </summary>
    /// <param name="featureId">The feature ID</param>
    /// <returns>Feature if found, null otherwise</returns>
    Task<Features?> GetFeatureAsync(string featureId);

    /// <summary>
    /// Gets features by type from cache
    /// </summary>
    /// <param name="type">The feature type</param>
    /// <returns>Collection of features of the specified type</returns>
    Task<IEnumerable<Features>> GetFeaturesByTypeAsync(string type);

    /// <summary>
    /// Gets all representative offices from cache
    /// </summary>
    /// <returns>Collection of representative offices</returns>
    Task<IEnumerable<Agency>> GetAgencyiesAsync();

    /// <summary>
    /// Gets a specific representative office by ID from cache
    /// </summary>
    /// <param name="officeId">The office ID</param>
    /// <returns>Representative office if found, null otherwise</returns>
    Task<Agency?> GetAgencyAsync(Guid officeId);

    /// <summary>
    /// Gets active representative offices from cache
    /// </summary>
    /// <returns>Collection of active representative offices</returns>
    Task<IEnumerable<Agency>> GetActiveAgencyiesAsync();

    /// <summary>
    /// Checks if a capability exists in cache
    /// </summary>
    /// <param name="capabilityId">The capability ID</param>
    /// <returns>True if capability exists, false otherwise</returns>
    Task<bool> CapabilityExistsAsync(string capabilityId);

    /// <summary>
    /// Checks if a feature exists in cache
    /// </summary>
    /// <param name="featureId">The feature ID</param>
    /// <returns>True if feature exists, false otherwise</returns>
    Task<bool> FeatureExistsAsync(string featureId);

    /// <summary>
    /// Checks if a representative office exists and is active in cache
    /// </summary>
    /// <param name="officeId">The office ID</param>
    /// <returns>True if office exists and is active, false otherwise</returns>
    Task<bool> AgencyExistsAsync(Guid officeId);

    /// <summary>
    /// Gets features for a specific capability from cache
    /// </summary>
    /// <param name="capabilityId">The capability ID</param>
    /// <returns>Collection of feature IDs for the capability</returns>
    Task<IEnumerable<string>> GetCapabilityFeaturesAsync(string capabilityId);

    /// <summary>
    /// Refreshes the cache with latest data from database
    /// </summary>
    /// <returns>Task representing the refresh operation</returns>
    Task RefreshCacheAsync();

    /// <summary>
    /// Updates cache when a capability is modified
    /// </summary>
    /// <param name="capability">The updated capability</param>
    /// <returns>Task representing the update operation</returns>
    Task UpdateCapabilityInCacheAsync(Capability capability);

    /// <summary>
    /// Updates cache when a feature is modified
    /// </summary>
    /// <param name="feature">The updated feature</param>
    /// <returns>Task representing the update operation</returns>
    Task UpdateFeatureInCacheAsync(Features feature);

    /// <summary>
    /// Updates cache when a representative office is modified
    /// </summary>
    /// <param name="office">The updated representative office</param>
    /// <returns>Task representing the update operation</returns>
    Task UpdateAgencyInCacheAsync(Agency office);

    /// <summary>
    /// Removes a capability from cache
    /// </summary>
    /// <param name="capabilityId">The capability ID to remove</param>
    /// <returns>Task representing the removal operation</returns>
    Task RemoveCapabilityFromCacheAsync(string capabilityId);

    /// <summary>
    /// Removes a feature from cache
    /// </summary>
    /// <param name="featureId">The feature ID to remove</param>
    /// <returns>Task representing the removal operation</returns>
    Task RemoveFeatureFromCacheAsync(string featureId);

    /// <summary>
    /// Removes a representative office from cache
    /// </summary>
    /// <param name="officeId">The office ID to remove</param>
    /// <returns>Task representing the removal operation</returns>
    Task RemoveAgencyFromCacheAsync(Guid officeId);

    /// <summary>
    /// Gets cache statistics
    /// </summary>
    /// <returns>Cache statistics</returns>
    Task<CacheStatistics> GetCacheStatisticsAsync();
}

/// <summary>
/// Cache statistics information
/// </summary>
public class CacheStatistics
{
    public int CapabilityCount { get; set; }
    public int FeatureCount { get; set; }
    public int AgencyCount { get; set; }
    public DateTime LastRefreshTime { get; set; }
    public TimeSpan CacheAge { get; set; }
    public bool IsCacheValid { get; set; }
}
