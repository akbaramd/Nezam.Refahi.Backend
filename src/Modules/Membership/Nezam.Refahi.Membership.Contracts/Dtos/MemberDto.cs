namespace Nezam.Refahi.Membership.Contracts.Dtos;

/// <summary>
/// Base Data Transfer Object for Member entity - used for lists and simple queries
/// Contains only raw data (primitive types) - no relational data
/// This DTO is domain-agnostic and suitable for list views
/// </summary>
public class MemberDto
{
    public Guid Id { get; set; }
    public string? MembershipNumber { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string NationalCode { get; set; } = string.Empty;
    public bool IsSpecial { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public DateTime? BirthDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    
    /// <summary>
    /// Gets the member's full name (computed property)
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";
    
    /// <summary>
    /// Checks if the member has an active membership (computed property)
    /// </summary>
    public bool HasActiveMembership => IsActive && !string.IsNullOrWhiteSpace(MembershipNumber);
}
