namespace Nezam.Refahi.Membership.Contracts.Dtos;

/// <summary>
/// External DTO for member response - includes capabilities with their claim assignments
/// </summary>
public class ExternalMemberResponseDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string NationalCode { get; set; } = string.Empty;
    public string MembershipCode { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<ExternalMemberCapabilityDto> Capabilities { get; set; } = new();
    public List<ExternalRoleDto> Roles { get; set; } = new();
    public List<ExternalLicenseDto> ActiveLicenses { get; set; } = new();

    /// <summary>
    /// Convenience property to get all claims from all capabilities
    /// </summary>
    public List<ExternalClaimDto> AllClaims =>
        Capabilities.SelectMany(c => c.Claims).ToList();

    public DateTime? Birthdate { get; set; }
}