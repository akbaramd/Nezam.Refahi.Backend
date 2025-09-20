namespace Nezam.Refahi.Membership.Contracts.Dtos;

/// <summary>
/// External DTO for claim type option - represents selectable values for Select/MultiSelect claim types
/// </summary>
public class ExternalClaimTypeOptionDto
{
    public string Value { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
}