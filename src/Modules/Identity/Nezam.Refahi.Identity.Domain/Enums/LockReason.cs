namespace Nezam.Refahi.Identity.Domain.Enums;

/// <summary>
/// Represents the reason for locking a user or challenge
/// </summary>
public enum LockReason
{
    /// <summary>
    /// No lock reason
    /// </summary>
    None = 0,
    
    /// <summary>
    /// Too many failed attempts
    /// </summary>
    TooManyAttempts = 1,
    
    /// <summary>
    /// Risk detected by security system
    /// </summary>
    RiskDetected = 2,
    
    /// <summary>
    /// Locked due to policy violation
    /// </summary>
    Policy = 3
}
