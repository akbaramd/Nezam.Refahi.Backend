namespace Nezam.Refahi.Facilities.Application.Dtos;

/// <summary>
/// Detailed facility data transfer object
/// Contains full facility information including all details
/// </summary>
public record FacilityDetailsDto : FacilityDto
{
  /// <summary>
  /// Bank name
  /// </summary>
  public string? BankName { get; init; }

  /// <summary>
  /// Bank code
  /// </summary>
  public string? BankCode { get; init; }

  /// <summary>
  /// Bank account number
  /// </summary>
  public string? BankAccountNumber { get; init; }

  /// <summary>
  /// Facility cycles
  /// </summary>
  public List<FacilityCycleWithUserDto> Cycles { get; init; } = new();

  /// <summary>
  /// Facility features
  /// </summary>
  public List<FacilityFeatureDto> Features { get; init; } = new();

  /// <summary>
  /// Capability policies
  /// </summary>
  public List<FacilityCapabilityPolicyDto> CapabilityPolicies { get; init; } = new();
}