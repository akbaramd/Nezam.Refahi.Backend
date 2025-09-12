namespace Nezam.Refahi.Identity.Contracts.Dtos;

/// <summary>
/// DTO for ValueObject: Claim
/// </summary>
public  class ClaimDto
{
  public string Type { get; set; } = string.Empty;
  public string Value { get; set; } = string.Empty;
  public string? ValueType { get; set; }
}