using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Shared.Infrastructure.Providers;

public interface IIdentityClaimsPermissionProvider
{
  public Task<IEnumerable<Claim>> GetAllClaimsAsync(CancellationToken cancellationToken = default);
}
