namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityDetails;

/// <summary>
/// Facility feature data transfer object
/// </summary>
public record FacilityFeatureDto
{
  /// <summary>
  /// Feature ID
  /// </summary>
  public Guid Id { get; init; }

  /// <summary>
  /// Feature ID
  /// </summary>
  public string FeatureId { get; init; } = null!;

  /// <summary>
  /// Requirement type
  /// </summary>
  public string RequirementType { get; init; } = null!;

  /// <summary>
  /// Notes
  /// </summary>
  public string? Notes { get; init; }

  /// <summary>
  /// Assigned date
  /// </summary>
  public DateTime AssignedAt { get; init; }
}