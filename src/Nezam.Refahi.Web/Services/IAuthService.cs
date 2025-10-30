using System.Security.Claims;

namespace Nezam.Refahi.Web.Services;

/// <summary>
/// Service for handling authentication and user information from HTTP context
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Gets the current authenticated user's information
    /// </summary>
    /// <returns>User authentication information or null if not authenticated</returns>
    UserAuthInfo? GetCurrentUser();

    /// <summary>
    /// Checks if the current user is authenticated
    /// </summary>
    /// <returns>True if user is authenticated, false otherwise</returns>
    bool IsAuthenticated();

    /// <summary>
    /// Gets the current user's roles
    /// </summary>
    /// <returns>List of role names</returns>
    List<string> GetUserRoles();

    /// <summary>
    /// Checks if the current user has a specific role
    /// </summary>
    /// <param name="roleName">Role name to check</param>
    /// <returns>True if user has the role, false otherwise</returns>
    bool HasRole(string roleName);

    /// <summary>
    /// Checks if the current user has any of the specified roles
    /// </summary>
    /// <param name="roleNames">Role names to check</param>
    /// <returns>True if user has any of the roles, false otherwise</returns>
    bool HasAnyRole(params string[] roleNames);

    /// <summary>
    /// Gets the current user's area access permissions
    /// </summary>
    /// <returns>Area access information</returns>
    AreaAccessInfo GetAreaAccess();
}

/// <summary>
/// User authentication information
/// </summary>
public class UserAuthInfo
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string NationalCode { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ProfileImagePath { get; set; } = "~/assets/images/users/avatar-1.jpg";
    public List<string> Roles { get; set; } = new();
    public string PrimaryRole { get; set; } = string.Empty;
}

/// <summary>
/// Area access information
/// </summary>
public class AreaAccessInfo
{
    public bool HasPanelAccess { get; set; }
    public bool HasEngineeringAccess { get; set; }
    public bool HasOwnerAccess { get; set; }
    public bool HasEmployerAccess { get; set; }
}
