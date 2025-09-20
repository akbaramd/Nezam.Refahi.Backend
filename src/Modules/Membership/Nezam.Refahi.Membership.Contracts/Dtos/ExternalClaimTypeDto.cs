namespace Nezam.Refahi.Membership.Contracts.Dtos;

/// <summary>
/// External DTO for claim type information - defines the structure of a claim
/// </summary>
public class ExternalClaimTypeDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ValueKind { get; set; } = string.Empty; // String, Number, Boolean, Select, MultiSelect
    public bool IsMultiValue { get; set; }
    public bool IsRequired { get; set; }
    public bool IsSystemManaged { get; set; }
    public string? ValidationRule { get; set; }
    public List<ExternalClaimTypeOptionDto> Options { get; set; } = new();
}