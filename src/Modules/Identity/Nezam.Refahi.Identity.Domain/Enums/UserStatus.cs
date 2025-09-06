namespace Nezam.Refahi.Identity.Domain.Enums;

/// <summary>
/// Represents the status of a user
/// </summary>
public enum UserStatus
{
    /// <summary>
    /// User is active and can authenticate
    /// </summary>
    Active = 0,
    
    /// <summary>
    /// User is locked and cannot authenticate
    /// </summary>
    Locked = 1
}
