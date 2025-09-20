using Nezam.Refahi.Shared.Domain.ValueObjects;
using Nezam.Refahi.Shared.Infrastructure.Providers;

namespace Nezam.Refahi.Identity.Infrastructure.Providers;

/// <summary>
/// Provides all identity-related claims and permissions for the system
/// </summary>
public class IdentityClaimsProvider : IIdentityClaimsPermissionProvider
{
    public Task<IEnumerable<Claim>> GetAllClaimsAsync(CancellationToken cancellationToken = default)
    {
        var claims = new List<Claim>
        {
            // ========================================================================
            // User Management Claims
            // ========================================================================
            new Claim("permission", "users.read", "string"),
            new Claim("permission", "users.write", "string"),
            new Claim("permission", "users.create", "string"),
            new Claim("permission", "users.update", "string"),
            new Claim("permission", "users.delete", "string"),
            new Claim("permission", "users.manage", "string"),
            new Claim("permission", "users.view_profile", "string"),
            new Claim("permission", "users.edit_profile", "string"),
            new Claim("permission", "users.change_password", "string"),
            new Claim("permission", "users.reset_password", "string"),
            new Claim("permission", "users.lock", "string"),
            new Claim("permission", "users.unlock", "string"),
            new Claim("permission", "users.activate", "string"),
            new Claim("permission", "users.deactivate", "string"),

            // ========================================================================
            // Role Management Claims
            // ========================================================================
            new Claim("permission", "roles.read", "string"),
            new Claim("permission", "roles.write", "string"),
            new Claim("permission", "roles.create", "string"),
            new Claim("permission", "roles.update", "string"),
            new Claim("permission", "roles.delete", "string"),
            new Claim("permission", "roles.manage", "string"),
            new Claim("permission", "roles.assign", "string"),
            new Claim("permission", "roles.unassign", "string"),

            // ========================================================================
            // Claims Management Claims
            // ========================================================================
            new Claim("permission", "claims.read", "string"),
            new Claim("permission", "claims.write", "string"),
            new Claim("permission", "claims.create", "string"),
            new Claim("permission", "claims.update", "string"),
            new Claim("permission", "claims.delete", "string"),
            new Claim("permission", "claims.manage", "string"),
            new Claim("permission", "claims.assign", "string"),
            new Claim("permission", "claims.unassign", "string"),

   

     
        };

        return Task.FromResult<IEnumerable<Claim>>(claims);
    }
}