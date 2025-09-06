namespace Nezam.Refahi.Identity.Domain.Enums;

/// <summary>
/// Represents the status of an OTP challenge
/// </summary>
public enum ChallengeStatus
{
    /// <summary>
    /// Challenge has been created and OTP generated
    /// </summary>
    Created = 0,
    
    /// <summary>
    /// OTP code has been sent to the user
    /// </summary>
    Sent = 1,
    
    /// <summary>
    /// OTP has been successfully verified
    /// </summary>
    Verified = 2,
    
    /// <summary>
    /// Challenge has been consumed (one-time use)
    /// </summary>
    Consumed = 3,
    
    /// <summary>
    /// Challenge has expired
    /// </summary>
    Expired = 4,
    
    /// <summary>
    /// Challenge is locked due to security reasons
    /// </summary>
    Locked = 5
}
