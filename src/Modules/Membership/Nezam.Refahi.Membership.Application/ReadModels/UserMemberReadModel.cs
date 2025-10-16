namespace Nezam.Refahi.Membership.Application.ReadModels;

/// <summary>
/// Read model for mapping User to Member relationship
/// Used for reverse lookups and reporting
/// </summary>
public class UserMemberReadModel
{
    public Guid UserId { get; set; }
    public Guid? MemberId { get; set; }
    public string? MembershipNumber { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? NationalId { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
}
