namespace Nezam.Refahi.Facilities.Application.Dtos;

/// <summary>
/// Facility capability policy data transfer object
/// </summary>
public class FacilityCapabilityPolicyDto
{
  /// <summary>
  /// Policy ID
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// Capability ID
  /// </summary>
  public string CapabilityId { get; set; } = null!;
}