using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Identity.Application.Services.Contracts;

/// <summary>
/// Service for aggregating claims from all registered IIdentityClaimsPermissionProvider implementations
/// </summary>
public interface IClaimsAggregatorService
{
  /// <summary>
  /// Gets all unique claims from all registered claims providers
  /// </summary>
  /// <param name="cancellationToken">Cancellation token</param>
  /// <returns>Distinct list of all available claims</returns>
  Task<IEnumerable<Claim>> GetAllDistinctClaimsAsync(CancellationToken cancellationToken = default);
}