using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.ValueObjects;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Identity.Domain.Rules;

/// <summary>
/// Business rules for role operations
/// </summary>
public static class RoleBusinessRules
{
    /// <summary>
    /// Checks if a role name is valid
    /// </summary>
    public static bool IsValidRoleName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        if (name.Length > 100)
            return false;

        // Check for reserved role names
        if (IsReservedRoleName(name))
            return false;

        // Check for valid characters (letters, numbers, spaces, hyphens, underscores)
        return name.All(c => char.IsLetterOrDigit(c) || c == ' ' || c == '-' || c == '_');
    }

    /// <summary>
    /// Checks if a role name is reserved
    /// </summary>
    public static bool IsReservedRoleName(string name)
    {
        var reservedNames = new[]
        {
            "SYSTEM",
            "ADMIN",
            "ROOT",
            "SUPERUSER",
            "ANONYMOUS",
            "GUEST"
        };

        return reservedNames.Contains(name.ToUpperInvariant());
    }

    /// <summary>
    /// Checks if a role can be modified
    /// </summary>
    public static bool CanModifyRole(Role role)
    {
        if (role == null)
            return false;

        return !role.IsSystemRole;
    }

    /// <summary>
    /// Checks if a role can be deleted
    /// </summary>
    public static bool CanDeleteRole(Role role)
    {
        if (role == null)
            return false;

        if (role.IsSystemRole)
            return false;

        return role.GetUserCount() == 0;
    }

    /// <summary>
    /// Checks if a role can be deactivated
    /// </summary>
    public static bool CanDeactivateRole(Role role)
    {
        if (role == null)
            return false;

        return !role.IsSystemRole;
    }

    /// <summary>
    /// Checks if a claim can be added to a role
    /// </summary>
    public static bool CanAddClaimToRole(Role role, Claim claim)
    {
        if (role == null || claim == null)
            return false;

        if (!role.IsActive)
            return false;

        // Check for duplicate claims
        if (role.HasClaim(claim))
            return false;

        // Check for valid claim
        if (!IsValidClaim(claim))
            return false;

        return true;
    }

    /// <summary>
    /// Checks if a claim can be removed from a role
    /// </summary>
    public static bool CanRemoveClaimFromRole(Role role, Claim claim)
    {
        if (role == null || claim == null)
            return false;

        if (role.IsSystemRole)
            return false;

        return role.HasClaim(claim);
    }

    /// <summary>
    /// Checks if a claim is valid
    /// </summary>
    public static bool IsValidClaim(Claim claim)
    {
        if (claim == null)
            return false;

        if (string.IsNullOrWhiteSpace(claim.Type))
            return false;

        if (string.IsNullOrWhiteSpace(claim.Value))
            return false;

        if (claim.Type.Length > 100)
            return false;

        if (claim.Value.Length > 1000)
            return false;

        return true;
    }

    /// <summary>
    /// Checks if a user can be assigned to a role
    /// </summary>
    public static bool CanAssignUserToRole(User user, Role role)
    {
        if (user == null || role == null)
            return false;

        if (!user.IsActive)
            return false;

        if (!role.IsActive)
            return false;

        if (user.IsLocked())
            return false;

        return true;
    }

    /// <summary>
    /// Checks if a user can be removed from a role
    /// </summary>
    public static bool CanRemoveUserFromRole(User user, Role role)
    {
        if (user == null || role == null)
            return false;

        if (role.IsSystemRole)
            return false;

        return true;
    }



    /// <summary>
    /// Gets the maximum number of claims a role can have
    /// </summary>
    public static int GetMaxClaimsPerRole(Role role)
    {
        return role.IsSystemRole ? 1000 : 100;
    }

    /// <summary>
    /// Checks if a role has reached the claim limit
    /// </summary>
    public static bool HasReachedClaimLimit(Role role, int currentClaimCount)
    {
        return currentClaimCount >= GetMaxClaimsPerRole(role);
    }

    /// <summary>
    /// Validates role assignment expiration
    /// </summary>
    public static bool IsValidExpirationDate(DateTime? expirationDate)
    {
        if (!expirationDate.HasValue)
            return true; // No expiration is valid

        return expirationDate.Value > DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if a role assignment is expired
    /// </summary>
    public static bool IsRoleAssignmentExpired(UserRole userRole)
    {
        if (userRole == null)
            return false;

        return userRole.IsExpired();
    }

    /// <summary>
    /// Checks if a role assignment will expire soon
    /// </summary>
    public static bool WillRoleAssignmentExpireSoon(UserRole userRole, TimeSpan timeThreshold)
    {
        if (userRole == null)
            return false;

        return userRole.WillExpireSoon(timeThreshold);
    }

    /// <summary>
    /// Gets the default expiration time for role assignments
    /// </summary>
    public static TimeSpan GetDefaultRoleAssignmentExpiration()
    {
        return TimeSpan.FromDays(365); // 1 year default
    }

    /// <summary>
    /// Checks if a role name conflicts with existing roles
    /// </summary>
    public static bool HasRoleNameConflict(string name, IEnumerable<Role> existingRoles, Guid? excludeRoleId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        var normalizedName = name.Trim().ToUpperInvariant();

        return existingRoles
            .Where(r => excludeRoleId == null || r.Id != excludeRoleId)
            .Any(r => r.Name.ToUpperInvariant() == normalizedName);
    }

    /// <summary>
    /// Validates role hierarchy (if implemented)
    /// </summary>
    public static bool IsValidRoleHierarchy(Role parentRole, Role childRole)
    {
        if (parentRole == null || childRole == null)
            return false;

        if (parentRole.Id == childRole.Id)
            return false; // Cannot be parent of itself

        // Add more hierarchy validation logic here if needed
        return true;
    }
}
