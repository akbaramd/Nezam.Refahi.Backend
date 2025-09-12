namespace Nezam.Refahi.Identity.Application.Features.Authentication.Commands.UpdateProfile;

/// <summary>
/// Response data for the UpdateProfileCommand
/// </summary>
public class UpdateProfileResponse
{
    /// <summary>
    /// The user's ID
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// The user's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// The user's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// The user's full name (first name + last name)
    /// </summary>
    public string FullName { get; set; } = string.Empty;
    
    /// <summary>
    /// The user's phone number (masked for security)
    /// </summary>
    public string MaskedPhoneNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// The user's national ID (masked for security)
    /// </summary>
    public string MaskedNationalId { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether the phone number was updated and requires re-verification
    /// </summary>
    public bool RequiresPhoneVerification { get; set; }
    
    /// <summary>
    /// Whether the user's profile is complete
    /// </summary>
    public bool IsProfileComplete { get; set; }
    
    /// <summary>
    /// The user's roles
    /// </summary>
    public IEnumerable<string> Roles { get; set; } = Array.Empty<string>();
    
    /// <summary>
    /// When the profile was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}