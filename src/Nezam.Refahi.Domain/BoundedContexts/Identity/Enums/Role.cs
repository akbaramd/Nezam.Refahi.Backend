using System;

namespace Nezam.Refahi.Domain.BoundedContexts.Identity.Enums;

/// <summary>
/// Represents the roles available in the Refahi system.
/// Roles are managed server-side and define the permissions for users.
/// </summary>
[Flags]
public enum Role
{
    /// <summary>
    /// No role assigned
    /// </summary>
    None = 0,
    
    /// <summary>
    /// Basic user role with minimal permissions
    /// </summary>
    User = 1 << 0,
    
    /// <summary>
    /// Editor role for content management
    /// </summary>
    Editor = 1 << 1,
    
    /// <summary>
    /// Manager role with elevated permissions
    /// </summary>
    Manager = 1 << 2,
    
    /// <summary>
    /// Administrator role with full system permissions
    /// </summary>
    Administrator = 1 << 3,
    
    /// <summary>
    /// Shorthand for all permissions
    /// </summary>
    All = User | Editor | Manager | Administrator
}
