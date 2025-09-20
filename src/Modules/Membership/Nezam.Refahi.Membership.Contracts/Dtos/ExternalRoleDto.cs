namespace Nezam.Refahi.Membership.Contracts.Dtos;

/// <summary>
/// External DTO for role information - roles define organizational hierarchy, not capabilities
/// </summary>
public class ExternalRoleDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public string? AssignedBy { get; set; }
    public DateTime AssignedAt { get; set; }
}