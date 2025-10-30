using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Nezam.Refahi.Web.Services;

/// <summary>
/// Implementation of authentication service that reads user information from HTTP context claims
/// </summary>
public class AuthService : IAuthService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    /// <summary>
    /// Gets the current authenticated user's information
    /// </summary>
    /// <returns>User authentication information or null if not authenticated</returns>
    public UserAuthInfo? GetCurrentUser()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated != true)
            return null;

        var user = httpContext.User;
        var claims = user.Claims.ToList();

        var userInfo = new UserAuthInfo
        {
            UserId = GetClaimValue(claims, ClaimTypes.NameIdentifier) ?? string.Empty,
            UserName = GetClaimValue(claims, ClaimTypes.Name) ?? string.Empty,
            FirstName = GetClaimValue(claims, ClaimTypes.GivenName) ?? string.Empty,
            LastName = GetClaimValue(claims, ClaimTypes.Surname) ?? string.Empty,
            PhoneNumber = GetClaimValue(claims, ClaimTypes.MobilePhone) ?? string.Empty,
            NationalCode = GetClaimValue(claims, "NationalCode") ?? string.Empty,
            Email = GetClaimValue(claims, ClaimTypes.Email) ?? string.Empty,
            ProfileImagePath = GetClaimValue(claims, "ProfileImagePath") ?? "~/assets/images/users/avatar-1.jpg"
        };

        // Build full name
        if (!string.IsNullOrEmpty(userInfo.FirstName) && !string.IsNullOrEmpty(userInfo.LastName))
        {
            userInfo.FullName = $"{userInfo.FirstName} {userInfo.LastName}";
        }
        else if (!string.IsNullOrEmpty(userInfo.UserName))
        {
            userInfo.FullName = userInfo.UserName;
        }
        else
        {
            userInfo.FullName = "کاربر سیستم";
        }

        // Get roles
        userInfo.Roles = GetUserRoles();
        userInfo.PrimaryRole = GetPrimaryRole(userInfo.Roles);

        return userInfo;
    }

    /// <summary>
    /// Checks if the current user is authenticated
    /// </summary>
    /// <returns>True if user is authenticated, false otherwise</returns>
    public bool IsAuthenticated()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        return httpContext?.User?.Identity?.IsAuthenticated == true;
    }

    /// <summary>
    /// Gets the current user's roles
    /// </summary>
    /// <returns>List of role names</returns>
    public List<string> GetUserRoles()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated != true)
            return new List<string>();

        return httpContext.User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();
    }

    /// <summary>
    /// Checks if the current user has a specific role
    /// </summary>
    /// <param name="roleName">Role name to check</param>
    /// <returns>True if user has the role, false otherwise</returns>
    public bool HasRole(string roleName)
    {
        if (string.IsNullOrEmpty(roleName))
            return false;

        var roles = GetUserRoles();
        return roles.Contains(roleName, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the current user has any of the specified roles
    /// </summary>
    /// <param name="roleNames">Role names to check</param>
    /// <returns>True if user has any of the roles, false otherwise</returns>
    public bool HasAnyRole(params string[] roleNames)
    {
        if (roleNames == null || roleNames.Length == 0)
            return false;

        var roles = GetUserRoles();
        return roleNames.Any(roleName => roles.Contains(roleName, StringComparer.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets the current user's area access permissions
    /// </summary>
    /// <returns>Area access information</returns>
    public AreaAccessInfo GetAreaAccess()
    {
        var roles = GetUserRoles();
        
        return new AreaAccessInfo
        {
            HasPanelAccess = HasAnyRole("Administrator", "Admin", "Manager"),
            HasEngineeringAccess = HasAnyRole("Administrator", "Admin", "Engineer", "Manager"),
            HasOwnerAccess = HasAnyRole("Administrator", "Admin", "Owner", "Manager"),
            HasEmployerAccess = HasAnyRole("Administrator", "Admin", "Employer", "Manager")
        };
    }

    /// <summary>
    /// Gets a claim value by type
    /// </summary>
    /// <param name="claims">List of claims</param>
    /// <param name="claimType">Claim type to find</param>
    /// <returns>Claim value or null if not found</returns>
    private static string? GetClaimValue(List<Claim> claims, string claimType)
    {
        return claims.FirstOrDefault(c => c.Type == claimType)?.Value;
    }

    /// <summary>
    /// Determines the primary role for display purposes
    /// </summary>
    /// <param name="roles">List of user roles</param>
    /// <returns>Primary role name</returns>
    private static string GetPrimaryRole(List<string> roles)
    {
        if (roles.Count == 0)
            return "کاربر";

        // Priority order for role display
        var rolePriority = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Administrator", "مدیر کل" },
            { "Admin", "مدیر" },
            { "Manager", "مدیر" },
            { "Engineer", "مهندس" },
            { "Owner", "مالک" },
            { "Employer", "کارمند" }
        };

        foreach (var role in roles)
        {
            if (rolePriority.TryGetValue(role, out var displayName))
            {
                return displayName;
            }
        }

        return roles.FirstOrDefault() ?? "کاربر";
    }
}
