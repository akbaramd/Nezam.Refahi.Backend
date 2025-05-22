using System;

namespace Nezam.Refahi.Application.Common.Interfaces;

/// <summary>
/// Service to access the current authenticated user's information
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current user's ID
    /// </summary>
    Guid? UserId { get; }
    
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
