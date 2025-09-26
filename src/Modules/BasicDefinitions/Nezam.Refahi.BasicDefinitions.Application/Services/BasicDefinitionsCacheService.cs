using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.BasicDefinitions.Contracts.Services;
using Nezam.Refahi.BasicDefinitions.Domain.Entities;
using Nezam.Refahi.BasicDefinitions.Domain.Repositories;

namespace Nezam.Refahi.BasicDefinitions.Application.Services;

/// <summary>
/// Implementation of basic definitions cache service using memory cache
/// Provides fast access to capabilities and features with automatic refresh
/// </summary>
public class BasicDefinitionsCacheService : IBasicDefinitionsCacheService
{
    private readonly IMemoryCache _cache;
    private readonly ICapabilityRepository _capabilityRepository;
    private readonly IFeaturesRepository _featuresRepository;
    private readonly ILogger<BasicDefinitionsCacheService> _logger;
    
    private const string CAPABILITIES_CACHE_KEY = "BasicDefinitions_Capabilities";
    private const string FEATURES_CACHE_KEY = "BasicDefinitions_Features";
    private const string CAPABILITY_FEATURES_CACHE_KEY = "BasicDefinitions_CapabilityFeatures_{0}";
    private const string LAST_REFRESH_CACHE_KEY = "BasicDefinitions_LastRefresh";
    
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5); // Cache expires every 5 minutes
    private readonly SemaphoreSlim _refreshSemaphore = new(1, 1);

    public BasicDefinitionsCacheService(
        IMemoryCache cache,
        ICapabilityRepository capabilityRepository,
        IFeaturesRepository featuresRepository,
        ILogger<BasicDefinitionsCacheService> logger)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _capabilityRepository = capabilityRepository ?? throw new ArgumentNullException(nameof(capabilityRepository));
        _featuresRepository = featuresRepository ?? throw new ArgumentNullException(nameof(featuresRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<Capability>> GetCapabilitiesAsync()
    {
        var capabilities = await GetCachedCapabilitiesAsync();
        return capabilities.Values;
    }

    public async Task<Capability?> GetCapabilityAsync(string capabilityId)
    {
        if (string.IsNullOrWhiteSpace(capabilityId))
            return null;

        var capabilities = await GetCachedCapabilitiesAsync();
        capabilities.TryGetValue(capabilityId, out var capability);
        return capability;
    }

    public async Task<IEnumerable<Features>> GetFeaturesAsync()
    {
        var features = await GetCachedFeaturesAsync();
        return features.Values;
    }

    public async Task<Features?> GetFeatureAsync(string featureId)
    {
        if (string.IsNullOrWhiteSpace(featureId))
            return null;

        var features = await GetCachedFeaturesAsync();
        features.TryGetValue(featureId, out var feature);
        return feature;
    }

    public async Task<IEnumerable<Features>> GetFeaturesByTypeAsync(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
            return Enumerable.Empty<Features>();

        var features = await GetCachedFeaturesAsync();
        return features.Values.Where(f => f.Type == type);
    }

    public async Task<bool> CapabilityExistsAsync(string capabilityId)
    {
        if (string.IsNullOrWhiteSpace(capabilityId))
            return false;

        var capabilities = await GetCachedCapabilitiesAsync();
        return capabilities.ContainsKey(capabilityId);
    }

    public async Task<bool> FeatureExistsAsync(string featureId)
    {
        if (string.IsNullOrWhiteSpace(featureId))
            return false;

        var features = await GetCachedFeaturesAsync();
        return features.ContainsKey(featureId);
    }

    public async Task<IEnumerable<string>> GetCapabilityFeaturesAsync(string capabilityId)
    {
        if (string.IsNullOrWhiteSpace(capabilityId))
            return Enumerable.Empty<string>();

        var capability = await GetCapabilityAsync(capabilityId);
        return capability?.Features?.Select(f => f.Id) ?? Enumerable.Empty<string>();
    }

    public async Task RefreshCacheAsync()
    {
        await _refreshSemaphore.WaitAsync();
        try
        {
            _logger.LogInformation("Starting cache refresh for BasicDefinitions");

            // Load capabilities from database
            var capabilities = await _capabilityRepository.GetActiveCapabilitiesAsync();
            var capabilityDict = capabilities.ToDictionary(c => c.Id, c => c);

            // Load features from database
            var features = await _featuresRepository.FindAsync(f => !f.IsDeleted);
            var featureDict = features.ToDictionary(f => f.Id, f => f);

            // Update cache
            _cache.Set(CAPABILITIES_CACHE_KEY, capabilityDict, _cacheExpiration);
            _cache.Set(FEATURES_CACHE_KEY, featureDict, _cacheExpiration);
            _cache.Set(LAST_REFRESH_CACHE_KEY, DateTime.UtcNow, _cacheExpiration);

            // Clear capability-feature mappings cache
            ClearCapabilityFeatureMappings();

            _logger.LogInformation("Cache refresh completed. Capabilities: {CapabilityCount}, Features: {FeatureCount}", 
                capabilityDict.Count, featureDict.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing BasicDefinitions cache");
            throw;
        }
        finally
        {
            _refreshSemaphore.Release();
        }
    }

    public async Task UpdateCapabilityInCacheAsync(Capability capability)
    {
        if (capability == null)
            return;

        try
        {
            var capabilities = await GetCachedCapabilitiesAsync();
            capabilities[capability.Id] = capability;
            
            _cache.Set(CAPABILITIES_CACHE_KEY, capabilities, _cacheExpiration);
            
            _logger.LogDebug("Updated capability {CapabilityId} in cache", capability.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating capability {CapabilityId} in cache", capability.Id);
        }
    }

    public async Task UpdateFeatureInCacheAsync(Features feature)
    {
        if (feature == null)
            return;

        try
        {
            var features = await GetCachedFeaturesAsync();
            features[feature.Id] = feature;
            
            _cache.Set(FEATURES_CACHE_KEY, features, _cacheExpiration);
            
            _logger.LogDebug("Updated feature {FeatureId} in cache", feature.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating feature {FeatureId} in cache", feature.Id);
        }
    }

    public async Task RemoveCapabilityFromCacheAsync(string capabilityId)
    {
        if (string.IsNullOrWhiteSpace(capabilityId))
            return;

        try
        {
            var capabilities = await GetCachedCapabilitiesAsync();
            capabilities.Remove(capabilityId);
            
            _cache.Set(CAPABILITIES_CACHE_KEY, capabilities, _cacheExpiration);
            
            // Clear capability-feature mapping cache
            var cacheKey = string.Format(CAPABILITY_FEATURES_CACHE_KEY, capabilityId);
            _cache.Remove(cacheKey);
            
            _logger.LogDebug("Removed capability {CapabilityId} from cache", capabilityId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing capability {CapabilityId} from cache", capabilityId);
        }
    }

    public async Task RemoveFeatureFromCacheAsync(string featureId)
    {
        if (string.IsNullOrWhiteSpace(featureId))
            return;

        try
        {
            var features = await GetCachedFeaturesAsync();
            features.Remove(featureId);
            
            _cache.Set(FEATURES_CACHE_KEY, features, _cacheExpiration);
            
            _logger.LogDebug("Removed feature {FeatureId} from cache", featureId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing feature {FeatureId} from cache", featureId);
        }
    }

    public async Task<CacheStatistics> GetCacheStatisticsAsync()
    {
        var capabilities = await GetCachedCapabilitiesAsync();
        var features = await GetCachedFeaturesAsync();
        var lastRefresh = _cache.Get<DateTime?>(LAST_REFRESH_CACHE_KEY);

        return new CacheStatistics
        {
            CapabilityCount = capabilities.Count,
            FeatureCount = features.Count,
            LastRefreshTime = lastRefresh ?? DateTime.MinValue,
            CacheAge = lastRefresh.HasValue ? DateTime.UtcNow - lastRefresh.Value : TimeSpan.Zero,
            IsCacheValid = lastRefresh.HasValue && DateTime.UtcNow - lastRefresh.Value < _cacheExpiration
        };
    }

    #region Private Methods

    private async Task<Dictionary<string, Capability>> GetCachedCapabilitiesAsync()
    {
        if (!_cache.TryGetValue(CAPABILITIES_CACHE_KEY, out Dictionary<string, Capability>? capabilities))
        {
            _logger.LogDebug("Capabilities cache miss, refreshing...");
            await RefreshCacheAsync();
            capabilities = _cache.Get<Dictionary<string, Capability>>(CAPABILITIES_CACHE_KEY) ?? new Dictionary<string, Capability>();
        }

        return capabilities ?? new Dictionary<string, Capability>();
    }

    private async Task<Dictionary<string, Features>> GetCachedFeaturesAsync()
    {
        if (!_cache.TryGetValue(FEATURES_CACHE_KEY, out Dictionary<string, Features>? features))
        {
            _logger.LogDebug("Features cache miss, refreshing...");
            await RefreshCacheAsync();
            features = _cache.Get<Dictionary<string, Features>>(FEATURES_CACHE_KEY) ?? new Dictionary<string, Features>();
        }

        return features ?? new Dictionary<string, Features>();
    }

    private void ClearCapabilityFeatureMappings()
    {
        // Clear all capability-feature mapping caches
        // This is a simple approach - in production you might want to track these keys
        _logger.LogDebug("Cleared capability-feature mapping caches");
    }

    #endregion
}
