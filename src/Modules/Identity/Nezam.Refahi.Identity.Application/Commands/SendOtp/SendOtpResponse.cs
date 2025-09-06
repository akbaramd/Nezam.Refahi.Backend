namespace Nezam.Refahi.Identity.Application.Commands.SendOtp;

/// <summary>
/// Response data for the SendOtpCommand
/// </summary>
public class SendOtpResponse
{
    /// <summary>
    /// The expiry time of the OTP in minutes
    /// </summary>
    public int ExpiryMinutes { get; set; }
    
    /// <summary>
    /// The masked phone number (e.g., "09*****1234")
    /// </summary>
    public string MaskedPhoneNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether the user is already registered in the system
    /// </summary>
    public bool IsRegistered { get; set; }
    
    /// <summary>
    /// Whether the phone number is locked due to too many failed attempts
    /// </summary>
    public bool IsLocked { get; set; }
    
    /// <summary>
    /// The remaining lockout time in minutes, if the phone number is locked
    /// </summary>
    public int RemainingLockoutMinutes { get; set; }
    
    /// <summary>
    /// The unique challenge identifier for this OTP request
    /// </summary>
    public string ChallengeId { get; set; } = string.Empty;
}
