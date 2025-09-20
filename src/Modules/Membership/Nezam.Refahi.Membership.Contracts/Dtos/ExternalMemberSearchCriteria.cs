namespace Nezam.Refahi.Membership.Contracts.Dtos;

public class ExternalMemberSearchCriteria
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? NationalCode { get; set; }
    public string? MembershipCode { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public List<string>? ServiceFields { get; set; }
    public List<string>? ServiceTypes { get; set; }
    public bool? IsActive { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}