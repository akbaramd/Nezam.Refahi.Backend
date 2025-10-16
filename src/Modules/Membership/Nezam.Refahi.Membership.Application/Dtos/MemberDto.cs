namespace Nezam.Refahi.Membership.Application.Dtos;

/// <summary>
/// Data Transfer Object for Member entity - used for inter-context communication
/// </summary>
public class MemberDto
{
    public Guid Id { get; set; }
    public string? MembershipNumber { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string NationalCode { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; }
    public DateTime? MembershipStartDate { get; set; }
    public DateTime? MembershipEndDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
    
    /// <summary>
    /// Gets the member's full name
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";
    
    /// <summary>
    /// Checks if the member has an active membership
    /// </summary>
    public bool HasActiveMembership => IsActive && (!MembershipEndDate.HasValue || DateTime.UtcNow <= MembershipEndDate.Value);
}