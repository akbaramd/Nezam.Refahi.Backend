using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Application.Services.Contracts;
using Nezam.Refahi.Shared.Domain.ValueObjects;
using Nezam.Refahi.Shared.Infrastructure.Providers;

namespace Nezam.Refahi.Identity.Infrastructure.Services;

/// <summary>
/// Service for aggregating claims from all registered IIdentityClaimsPermissionProvider implementations
/// </summary>
public class ClaimsAggregatorService : IClaimsAggregatorService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ClaimsAggregatorService> _logger;

    public ClaimsAggregatorService(
        IServiceProvider serviceProvider,
        ILogger<ClaimsAggregatorService> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets all unique claim values from all registered claims providers
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Distinct list of all available claim values</returns>
    public async Task<IEnumerable<Claim>> GetAllDistinctClaimsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Get all registered IIdentityClaimsPermissionProvider implementations
            var claimsProviders = _serviceProvider.GetServices<IIdentityClaimsPermissionProvider>();
            
            if (!claimsProviders.Any())
            {
                _logger.LogWarning("No IIdentityClaimsPermissionProvider implementations found in service provider");
                return Enumerable.Empty<Claim>();
            }

            var allClaimValues = new HashSet<Claim>();
            var providerCount = 0;

            // Aggregate claims from all providers
            foreach (var provider in claimsProviders)
            {
                try
                {
                    var claims = await provider.GetAllClaimsAsync(cancellationToken);
                    
                    foreach (var claim in claims ?? Enumerable.Empty<Claim>())
                    {
                        if (!string.IsNullOrWhiteSpace(claim?.Value))
                        {
                            allClaimValues.Add(claim);
                        }
                    }
                    
                    providerCount++;
                    _logger.LogDebug("Successfully aggregated claims from provider {ProviderType}", 
                        provider.GetType().Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to get claims from provider {ProviderType}", 
                        provider.GetType().Name);
                    // Continue with other providers even if one fails
                }
            }

            _logger.LogInformation("Successfully aggregated {ClaimCount} distinct claim values from {ProviderCount} providers", 
                allClaimValues.Count, providerCount);

            return allClaimValues.OrderBy(x => x);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to aggregate claims from providers");
            throw;
        }
    }

    
}