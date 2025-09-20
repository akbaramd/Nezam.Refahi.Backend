namespace Nezam.Refahi.Membership.Contracts.Dtos;

/// <summary>
/// External DTO for claim value - represents an actual claim value assigned to a member
/// </summary>
public class ExternalClaimDto
{
    public Guid ClaimTypeId { get; set; }
    public string ClaimTypeKey { get; set; } = string.Empty;
    public string ClaimTypeTitle { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string ValueKind { get; set; } = string.Empty; // String, Number, Boolean, Select, MultiSelect
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public string? AssignedBy { get; set; }
    public DateTime AssignedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
}