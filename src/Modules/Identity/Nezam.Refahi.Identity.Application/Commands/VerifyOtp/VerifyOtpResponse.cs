namespace Nezam.Refahi.Identity.Application.Commands.VerifyOtp;

/// <summary>
/// Response data for the VerifyOtpCommand
/// </summary>
public class VerifyOtpResponse
{
    /// <summary>
    /// The user's ID
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// The access token (JWT)
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;
    
    /// <summary>
    /// The refresh token
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
    
    /// <summary>
    /// The expiry time of the access token in minutes
    /// </summary>
    public int ExpiryMinutes { get; set; }
    
    /// <summary>
    /// Whether the user is already registered in the system
    /// </summary>
    public bool IsRegistered { get; set; }
    
    /// <summary>
    /// Indicates whether the user needs to complete their registration by providing additional information
    /// </summary>
    public bool RequiresRegistrationCompletion { get; set; }
}
