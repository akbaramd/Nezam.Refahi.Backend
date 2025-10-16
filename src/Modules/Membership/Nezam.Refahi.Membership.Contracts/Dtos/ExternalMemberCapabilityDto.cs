namespace Nezam.Refahi.Membership.Contracts.Dtos;

/// <summary>
/// External DTO for member capability assignment - represents the relationship between member and capability with assignment metadata
/// </summary>
public class ExternalMemberCapabilityDto
{
    public ExternalCapabilityDto Capability { get; set; } = new();
    public bool IsActive { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public string? AssignedBy { get; set; }
    public DateTime AssignedAt { get; set; }
    public string? Notes { get; set; }
    public List<ExternalClaimDto> Claims { get; set; } = new(); // Actual claim values for this capability assignment
}