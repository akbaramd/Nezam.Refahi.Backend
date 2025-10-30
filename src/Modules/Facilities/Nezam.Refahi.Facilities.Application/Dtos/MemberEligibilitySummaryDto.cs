namespace Nezam.Refahi.Facilities.Application.Dtos;

/// <summary>
/// Member eligibility summary data transfer object
/// </summary>
public record MemberEligibilitySummaryDto
{
  /// <summary>
  /// Total number of facilities
  /// </summary>
  public int TotalFacilities { get; init; }

  /// <summary>
  /// Number of eligible facilities
  /// </summary>
  public int EligibleFacilities { get; init; }

  /// <summary>
  /// Number of ineligible facilities
  /// </summary>
  public int IneligibleFacilities { get; init; }

  /// <summary>
  /// Eligibility percentage
  /// </summary>
  public decimal EligibilityPercentage { get; init; }

  /// <summary>
  /// Total number of active cycles
  /// </summary>
  public int TotalActiveCycles { get; init; }

  /// <summary>
  /// Number of eligible active cycles
  /// </summary>
  public int EligibleActiveCycles { get; init; }

  /// <summary>
  /// Number of cycles with available quota
  /// </summary>
  public int CyclesWithAvailableQuota { get; init; }

  /// <summary>
  /// Number of cycles user has already requested
  /// </summary>
  public int CyclesAlreadyRequested { get; init; }

  /// <summary>
  /// Number of cycles user can still request
  /// </summary>
  public int CyclesCanStillRequest { get; init; }

  /// <summary>
  /// Common eligibility issues
  /// </summary>
  public List<string> CommonEligibilityIssues { get; init; } = new();

  /// <summary>
  /// Missing features across all facilities
  /// </summary>
  public List<string> MissingFeatures { get; init; } = new();

  /// <summary>
  /// Missing capabilities across all facilities
  /// </summary>
  public List<string> MissingCapabilities { get; init; } = new();
}