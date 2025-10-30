namespace Nezam.Refahi.Facilities.Application.Dtos;

/// <summary>
/// Facility feature data transfer object
/// </summary>
public class FacilityFeatureDto
{
  /// <summary>
  /// Feature ID
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// Feature ID
  /// </summary>
  public string FeatureId { get; set; } = null!;

  /// <summary>
  /// Requirement type
  /// </summary>
  public string RequirementType { get; set; } = null!;

  /// <summary>
  /// Notes
  /// </summary>
  public string? Notes { get; set; }

  /// <summary>
  /// Assigned date
  /// </summary>
  public DateTime AssignedAt { get; set; }
}