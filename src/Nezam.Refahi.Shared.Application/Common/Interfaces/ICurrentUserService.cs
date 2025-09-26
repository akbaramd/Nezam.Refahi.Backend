namespace Nezam.Refahi.Shared.Application.Common.Interfaces;


public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current user's ID
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// Gets the current user's national number
    /// </summary>
    string? UserNationalNumber { get; }

    /// <summary>
    /// Gets the current user's full name
    /// </summary>
    string? UserFullName { get; }

    /// <summary>
    /// Checks if the current user is authenticated
    /// </summary>
    bool IsAuthenticated { get; }
    
    /// <summary>
    /// Gets the current user's roles
    /// </summary>
    IEnumerable<string> Roles { get; }
    
    /// <summary>
    /// Checks if the current user has a specific role
    /// </summary>
    /// <param name="role">The role to check</param>
    bool HasRole(string role);
}
