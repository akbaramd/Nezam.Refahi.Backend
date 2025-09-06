namespace Nezam.Refahi.Identity.Application.Queries.GetCurrentUser;

/// <summary>
/// Simple, professional response model for the current user - like Google/Facebook APIs
/// </summary>
public class CurrentUserResponse
{
    /// <summary>
    /// User ID
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// User's full name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// User's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User's national ID
    /// </summary>
    public string NationalId { get; set; } = string.Empty;
    
    /// <summary>
    /// User's phone number
    /// </summary>
    public string Phone { get; set; } = string.Empty;
    
    /// <summary>
    /// User's roles
    /// </summary>
    public IEnumerable<string> Roles { get; set; } = Array.Empty<string>();
    
    /// <summary>
    /// User's claims
    /// </summary>
    public IEnumerable<ClaimDto> Claims { get; set; } = Array.Empty<ClaimDto>();
    
    /// <summary>
    /// User's preferences
    /// </summary>
    public IEnumerable<PreferenceDto> Preferences { get; set; } = Array.Empty<PreferenceDto>();
}

/// <summary>
/// Simple claim DTO
/// </summary>
public class ClaimDto
{
    /// <summary>
    /// Claim value
    /// </summary>
    public string Value { get; set; } = string.Empty;
    
    /// <summary>
    /// Claim type (permission, role, scope, etc.)
    /// </summary>
    public string Type { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether the claim is active
    /// </summary>
    public bool IsActive { get; set; }
}

/// <summary>
/// Simple preference DTO
/// </summary>
public class PreferenceDto
{
    /// <summary>
    /// Preference key
    /// </summary>
    public string Key { get; set; } = string.Empty;
    
    /// <summary>
    /// Preference value
    /// </summary>
    public string Value { get; set; } = string.Empty;
    
    /// <summary>
    /// Preference category
    /// </summary>
    public string Category { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether the preference is active
    /// </summary>
    public bool IsActive { get; set; }
}

