namespace Nezam.Refahi.Membership.Contracts.Dtos;

/// <summary>
/// Data Transfer Object for Member information - used for inter-context communication
/// This DTO is used by other bounded contexts to access member information
/// </summary>
public class MemberInfoDto
{
    public Guid Id { get; set; }
    public string? MembershipNumber { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string NationalCode { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public DateTime? BirthDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    
    /// <summary>
    /// Member capabilities (string keys)
    /// </summary>
    public List<string> Capabilities { get; set; } = new();
    
    /// <summary>
    /// Member features (string keys)
    /// </summary>
    public List<string> Features { get; set; } = new();
    
    /// <summary>
    /// Gets the member's full name
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";
    
    /// <summary>
    /// Checks if the member has an active membership
    /// </summary>
    public bool HasActiveMembership => IsActive && MembershipNumber != null;
}
