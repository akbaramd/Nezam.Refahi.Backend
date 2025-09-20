namespace Nezam.Refahi.Membership.Contracts.Dtos;

/// <summary>
/// External DTO for capability information - contains a collection of claim types
/// </summary>
public class ExternalCapabilityDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public List<ExternalClaimTypeDto> ClaimTypes { get; set; } = new();
}