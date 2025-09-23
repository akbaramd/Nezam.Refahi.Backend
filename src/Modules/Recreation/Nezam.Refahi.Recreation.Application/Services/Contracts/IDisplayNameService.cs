namespace Nezam.Refahi.Recreation.Application.Services.Contracts;

/// <summary>
/// Service for mapping internal IDs to user-friendly display names
/// </summary>
public interface IDisplayNameService
{
    /// <summary>
    /// Gets display name for a capability ID
    /// </summary>
    /// <param name="capabilityId">Internal capability identifier</param>
    /// <returns>Persian display name for the capability</returns>
    string GetCapabilityDisplayName(string capabilityId);

    /// <summary>
    /// Gets display name for a feature ID
    /// </summary>
    /// <param name="featureId">Internal feature identifier</param>
    /// <returns>Persian display name for the feature</returns>
    string GetFeatureDisplayName(string featureId);

    /// <summary>
    /// Gets all available capability mappings
    /// </summary>
    /// <returns>Dictionary of capability ID to display name mappings</returns>
    IDictionary<string, string> GetAllCapabilityMappings();

    /// <summary>
    /// Gets all available feature mappings
    /// </summary>
    /// <returns>Dictionary of feature ID to display name mappings</returns>
    IDictionary<string, string> GetAllFeatureMappings();
}
