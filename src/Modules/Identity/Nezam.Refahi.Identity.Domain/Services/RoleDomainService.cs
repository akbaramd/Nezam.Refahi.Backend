using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.ValueObjects;
using Nezam.Refahi.Identity.Domain.Rules;
using Nezam.Refahi.Identity.Domain.Events;

namespace Nezam.Refahi.Identity.Domain.Services;

/// <summary>
/// Stateless domain service for role-related business logic
/// This service contains pure business logic without any dependencies
/// </summary>
public static class RoleDomainService
{
    /// <summary>
    /// Validates if a role can be created with the given name
    /// </summary>
    public static ValidationResult ValidateRoleCreation(string name, string? description, IEnumerable<Role> existingRoles)
    {
        var result = new ValidationResult();

        if (!RoleBusinessRules.IsValidRoleName(name))
        {
            result.AddError("Invalid role name format");
        }

        if (RoleBusinessRules.HasRoleNameConflict(name, existingRoles))
        {
            result.AddError($"Role name '{name}' already exists");
        }

        if (description != null && description.Length > 500)
        {
            result.AddError("Description cannot exceed 500 characters");
        }

        return result;
    }

    /// <summary>
    /// Validates if a role can be updated
    /// </summary>
    public static ValidationResult ValidateRoleUpdate(Role role, string newName, string? newDescription, IEnumerable<Role> existingRoles)
    {
        var result = new ValidationResult();

        if (!RoleBusinessRules.CanModifyRole(role))
        {
            result.AddError("Cannot modify system role");
        }

        if (!RoleBusinessRules.IsValidRoleName(newName))
        {
            result.AddError("Invalid role name format");
        }

        if (RoleBusinessRules.HasRoleNameConflict(newName, existingRoles, role.Id))
        {
            result.AddError($"Role name '{newName}' already exists");
        }

        if (newDescription != null && newDescription.Length > 500)
        {
            result.AddError("Description cannot exceed 500 characters");
        }

        return result;
    }

    /// <summary>
    /// Validates if a role can be deleted
    /// </summary>
    public static ValidationResult ValidateRoleDeletion(Role role)
    {
        var result = new ValidationResult();

        if (!RoleBusinessRules.CanDeleteRole(role))
        {
            if (role.IsSystemRole)
            {
                result.AddError("Cannot delete system role");
            }
            else if (role.GetUserCount() > 0)
            {
                result.AddError("Cannot delete role that has assigned users");
            }
        }

        return result;
    }

    /// <summary>
    /// Validates if a claim can be added to a role
    /// </summary>
    public static ValidationResult ValidateClaimAddition(Role role, Claim claim)
    {
        var result = new ValidationResult();

        if (!RoleBusinessRules.CanAddClaimToRole(role, claim))
        {
            if (!role.IsActive)
            {
                result.AddError("Cannot add claims to inactive role");
            }
            else if (role.HasClaim(claim))
            {
                result.AddError("Claim already exists in role");
            }
            else if (!RoleBusinessRules.IsValidClaim(claim))
            {
                result.AddError("Invalid claim format");
            }
        }

        if (RoleBusinessRules.HasReachedClaimLimit(role, role.Claims.Count))
        {
            result.AddError("Role has reached maximum claim limit");
        }

        return result;
    }

    /// <summary>
    /// Validates if a claim can be removed from a role
    /// </summary>
    public static ValidationResult ValidateClaimRemoval(Role role, Claim claim)
    {
        var result = new ValidationResult();

        if (!RoleBusinessRules.CanRemoveClaimFromRole(role, claim))
        {
            if (role.IsSystemRole)
            {
                result.AddError("Cannot remove claims from system role");
            }
            else if (!role.HasClaim(claim))
            {
                result.AddError("Claim does not exist in role");
            }
        }

        return result;
    }

    /// <summary>
    /// Validates if a user can be assigned to a role
    /// </summary>
    public static ValidationResult ValidateUserRoleAssignment(User user, Role role, int currentUserRoleCount)
    {
        var result = new ValidationResult();

        if (!RoleBusinessRules.CanAssignUserToRole(user, role))
        {
            if (!user.IsActive)
            {
                result.AddError("Cannot assign role to inactive user");
            }
            else if (!role.IsActive)
            {
                result.AddError("Cannot assign inactive role to user");
            }
            else if (user.IsLocked())
            {
                result.AddError("Cannot assign role to locked user");
            }
        }

      

        if (user.HasRole(role.Id))
        {
            result.AddError("User already has this role");
        }

        return result;
    }

    /// <summary>
    /// Validates if a user can be removed from a role
    /// </summary>
    public static ValidationResult ValidateUserRoleRemoval(User user, Role role)
    {
        var result = new ValidationResult();

        if (!RoleBusinessRules.CanRemoveUserFromRole(user, role))
        {
            if (role.IsSystemRole)
            {
                result.AddError("Cannot remove user from system role");
            }
        }

        if (!user.HasRole(role.Id))
        {
            result.AddError("User does not have this role");
        }

        return result;
    }

    /// <summary>
    /// Validates role assignment expiration
    /// </summary>
    public static ValidationResult ValidateRoleAssignmentExpiration(DateTime? expirationDate)
    {
        var result = new ValidationResult();

        if (!RoleBusinessRules.IsValidExpirationDate(expirationDate))
        {
            result.AddError("Expiration date must be in the future");
        }

        return result;
    }

    /// <summary>
    /// Gets all claims for a user through their roles
    /// </summary>
    public static IEnumerable<Claim> GetUserClaims(User user)
    {
        return user.GetActiveRoles()
            .SelectMany(ur => ur.Role.Claims)
            .Select(rc => rc.Claim)
            .Distinct();
    }

    /// <summary>
    /// Gets claims of a specific type for a user
    /// </summary>
    public static IEnumerable<Claim> GetUserClaimsByType(User user, string claimType)
    {
        return GetUserClaims(user)
            .Where(c => c.Type.Equals(claimType, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Checks if a user has a specific claim
    /// </summary>
    public static bool UserHasClaim(User user, Claim claim)
    {
        return GetUserClaims(user).Any(c => c.Type == claim.Type && c.Value == claim.Value);
    }

    /// <summary>
    /// Checks if a user has a claim by type and value
    /// </summary>
    public static bool UserHasClaim(User user, string claimType, string claimValue)
    {
        return GetUserClaims(user).Any(c => c.Type == claimType && c.Value == claimValue);
    }

    /// <summary>
    /// Gets all permission claims for a user
    /// </summary>
    public static IEnumerable<Claim> GetUserPermissions(User user)
    {
        return GetUserClaimsByType(user, "permission");
    }

    /// <summary>
    /// Checks if a user has a specific permission
    /// </summary>
    public static bool UserHasPermission(User user, string permission)
    {
        return UserHasClaim(user, "permission", permission);
    }

    /// <summary>
    /// Gets all scope claims for a user
    /// </summary>
    public static IEnumerable<Claim> GetUserScopes(User user)
    {
        return GetUserClaimsByType(user, "scope");
    }

    /// <summary>
    /// Checks if a user has a specific scope
    /// </summary>
    public static bool UserHasScope(User user, string scope)
    {
        return UserHasClaim(user, "scope", scope);
    }

    /// <summary>
    /// Creates default system roles with their claims
    /// </summary>
    public static IEnumerable<(Role Role, IEnumerable<Claim> Claims)> CreateDefaultSystemRoles()
    {
        var roles = new List<(Role Role, IEnumerable<Claim> Claims)>();

        // Admin Role
        var adminRole = new Role("Admin", "System administrator with full access", isSystemRole: true, displayOrder: 1);
        var adminClaims = new[]
        {
            Claim.CreatePermission("users.read"),
            Claim.CreatePermission("users.create"),
            Claim.CreatePermission("users.update"),
            Claim.CreatePermission("users.delete"),
            Claim.CreatePermission("roles.read"),
            Claim.CreatePermission("roles.create"),
            Claim.CreatePermission("roles.update"),
            Claim.CreatePermission("roles.delete"),
            Claim.CreatePermission("system.admin"),
            Claim.CreateScope("admin")
        };
        roles.Add((adminRole, adminClaims));

        // Moderator Role
        var moderatorRole = new Role("Moderator", "Content moderator with limited admin access", isSystemRole: true, displayOrder: 2);
        var moderatorClaims = new[]
        {
            Claim.CreatePermission("users.read"),
            Claim.CreatePermission("users.update"),
            Claim.CreatePermission("content.moderate"),
            Claim.CreateScope("moderator")
        };
        roles.Add((moderatorRole, moderatorClaims));

        // User Role
        var userRole = new Role("User", "Standard user with basic access", isSystemRole: true, displayOrder: 3);
        var userClaims = new[]
        {
            Claim.CreatePermission("profile.read"),
            Claim.CreatePermission("profile.update"),
            Claim.CreateScope("user")
        };
        roles.Add((userRole, userClaims));

        return roles;
    }
}

/// <summary>
/// Simple validation result class for domain service operations
/// </summary>
public class ValidationResult
{
    private readonly List<string> _errors = new();

    public bool IsValid => !_errors.Any();
    public IReadOnlyList<string> Errors => _errors.AsReadOnly();

    public void AddError(string error)
    {
        if (!string.IsNullOrWhiteSpace(error))
        {
            _errors.Add(error);
        }
    }

    public void AddErrors(IEnumerable<string> errors)
    {
        foreach (var error in errors)
        {
            AddError(error);
        }
    }

    public string GetErrorMessage()
    {
        return string.Join("; ", _errors);
    }
}
