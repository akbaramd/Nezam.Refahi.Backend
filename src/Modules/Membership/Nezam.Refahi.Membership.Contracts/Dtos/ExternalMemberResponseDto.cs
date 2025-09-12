namespace Nezam.Refahi.Membership.Contracts.Dtos;

public class ExternalMemberResponseDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string NationalCode { get; set; } = string.Empty;
    public string MembershipCode { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<ExternalClaimDto> Claims { get; set; } = new();
    public List<ExternalRoleDto> Roles { get; set; } = new();
    public List<ExternalLicenseDto> ActiveLicenses { get; set; } = new();
}