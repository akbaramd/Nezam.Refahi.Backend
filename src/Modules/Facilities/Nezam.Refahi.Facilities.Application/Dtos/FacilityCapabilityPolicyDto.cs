namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityDetails;

/// <summary>
/// Facility capability policy data transfer object
/// </summary>
public record FacilityCapabilityPolicyDto
{
  /// <summary>
  /// Policy ID
  /// </summary>
  public Guid Id { get; init; }

  /// <summary>
  /// Policy type
  /// </summary>
  public string PolicyType { get; init; } = null!;

  /// <summary>
  /// Policy value
  /// </summary>
  public string PolicyValue { get; init; } = null!;

  /// <summary>
  /// Notes
  /// </summary>
  public string? Notes { get; init; }

  /// <summary>
  /// Assigned date
  /// </summary>
  public DateTime AssignedAt { get; init; }
}