using Nezam.Refahi.Identity.Contracts.Dtos;

namespace Nezam.Refahi.Identity.Application.Features.Authentication.Queries.GetCurrentUser;

/// <summary>
/// Simple, professional response model for the current user - like Google/Facebook APIs
/// </summary>
public class CurrentUserResponse
{
    /// <summary>
    /// UserDetail ID
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// UserDetail's full name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// UserDetail's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// UserDetail's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// UserDetail's national ID
    /// </summary>
    public string NationalId { get; set; } = string.Empty;
    
    /// <summary>
    /// UserDetail's phone number
    /// </summary>
    public string Phone { get; set; } = string.Empty;
    
    /// <summary>
    /// UserDetail's roles
    /// </summary>
    public IEnumerable<string> Roles { get; set; } = Array.Empty<string>();
    
    /// <summary>
    /// UserDetail's claims
    /// </summary>
    public IEnumerable<ClaimDto> Claims { get; set; } = Array.Empty<ClaimDto>();
    
    /// <summary>
    /// UserDetail's preferences
    /// </summary>
    public IEnumerable<UserPreferenceDto> Preferences { get; set; } = Array.Empty<UserPreferenceDto>();
}