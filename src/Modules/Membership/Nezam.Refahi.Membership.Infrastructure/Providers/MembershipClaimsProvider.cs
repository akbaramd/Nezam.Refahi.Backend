using Nezam.Refahi.Shared.Domain.ValueObjects;
using Nezam.Refahi.Shared.Infrastructure.Providers;

namespace Nezam.Refahi.Membership.Infrastructure.Providers;

/// <summary>
/// Provides all membership-related claims and permissions for the system
/// </summary>
public class MembershipClaimsProvider : IIdentityClaimsPermissionProvider
{
    public Task<IEnumerable<Claim>> GetAllClaimsAsync(CancellationToken cancellationToken = default)
    {
        var claims = new List<Claim>
        {
            // ========================================================================
            // Member Management Claims
            // ========================================================================
            new Claim("permission", "members.read", "string"),
            new Claim("permission", "members.write", "string"),
            new Claim("permission", "members.create", "string"),
            new Claim("permission", "members.update", "string"),
            new Claim("permission", "members.delete", "string"),
            new Claim("permission", "members.manage", "string"),
            new Claim("permission", "members.search", "string"),
            new Claim("permission", "members.export", "string"),
            new Claim("permission", "members.import", "string"),
            new Claim("permission", "members.archive", "string"),
            new Claim("permission", "members.restore", "string"),
            new Claim("permission", "members.approve", "string"),
            new Claim("permission", "members.reject", "string"),
            new Claim("permission", "members.suspend", "string"),
            new Claim("permission", "members.reactivate", "string"),

    
        };

        return Task.FromResult<IEnumerable<Claim>>(claims);
    }
}