using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserMemberInfo;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;

/// <summary>
/// Response for facility cycle details query
/// </summary>
public record GetFacilityCycleDetailsResponse
{
  /// <summary>
  /// Detailed cycle information
  /// </summary>
  public FacilityCycleDetailsDto Cycle { get; init; } = null!;

  /// <summary>
  /// Facility information (if requested)
  /// </summary>
  public FacilityInfoDto? Facility { get; init; }

  /// <summary>
  /// User member information (if NationalNumber provided)
  /// </summary>
  public UserMemberInfoDto? UserInfo { get; init; }

  /// <summary>
  /// Detailed user eligibility analysis (if NationalNumber provided)
  /// </summary>
  public DetailedEligibilityDto? EligibilityAnalysis { get; init; }

  /// <summary>
  /// User request history for this cycle (if NationalNumber provided)
  /// </summary>
  public List<UserRequestHistoryDto> UserRequestHistory { get; init; } = new();

  /// <summary>
  /// Last request for this cycle (if user has requests)
  /// </summary>
  public UserRequestHistoryDto? LastRequest { get; init; }

  /// <summary>
  /// Cycle dependencies (if requested)
  /// </summary>
  public List<CycleDependencyDto>? Dependencies { get; init; }

  /// <summary>
  /// Cycle statistics (if requested)
  /// </summary>
  public CycleStatisticsDto? Statistics { get; init; }
}