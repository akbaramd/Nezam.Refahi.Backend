namespace Nezam.Refahi.Application.Features.Auth.Queries.GetCurrentUser;

/// <summary>
/// Response model for the current user's profile
/// </summary>
public class CurrentUserResponse
{
 /// <summary>
    /// The user's ID
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// The user's phone number
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// The user's full name (first name + last name)
    /// </summary>
    public string FullName { get; set; } = string.Empty;
    
    /// <summary>
    /// The user's roles
    /// </summary>
    public IEnumerable<string> Roles { get; set; } = Array.Empty<string>();
    
    /// <summary>
    /// Whether the user is authenticated
    /// </summary>
    public bool IsAuthenticated { get; set; }
    
    /// <summary>
    /// The user's national ID (masked for security)
    /// </summary>
    public string MaskedNationalId { get; set; } = string.Empty;
    
    /// <summary>
    /// The user's profile completion status
    /// </summary>
    public bool IsProfileComplete { get; set; }
}
