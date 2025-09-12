namespace Nezam.Refahi.Membership.Contracts.Dtos;

public class ExternalLicenseDto
{
    public string LicenseNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime ExpireDate { get; set; }
    public DateTime ExtensionDate { get; set; }
    public bool IsActive { get; set; }
    public List<ExternalClaimDto> Claims { get; set; } = new();
}