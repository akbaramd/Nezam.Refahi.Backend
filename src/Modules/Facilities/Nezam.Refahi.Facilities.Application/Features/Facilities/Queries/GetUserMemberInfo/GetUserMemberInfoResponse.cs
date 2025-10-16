using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserMemberInfo;

/// <summary>
/// Response for user member info query
/// </summary>
public record GetUserMemberInfoResponse
{
  /// <summary>
  /// User member information
  /// </summary>
  public UserMemberInfoDto MemberInfo { get; init; } = null!;

  /// <summary>
  /// Member request statistics (if requested)
  /// </summary>
  public MemberRequestStatisticsDto? RequestStatistics { get; init; }

  /// <summary>
  /// Member eligibility summary (if requested)
  /// </summary>
  public MemberEligibilitySummaryDto? EligibilitySummary { get; init; }

  /// <summary>
  /// Member profile completeness (if requested)
  /// </summary>
  public MemberProfileCompletenessDto? ProfileCompleteness { get; init; }
}