using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserMemberInfo;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserMemberInfo;

/// <summary>
/// Detailed user member information data transfer object
/// Contains full member information including capabilities and features
/// </summary>
public record UserMemberInfoDetailsDto : UserMemberInfoDto
{
  /// <summary>
  /// Member capabilities
  /// </summary>
  public List<MemberCapabilityDto> Capabilities { get; init; } = new();

  /// <summary>
  /// Member features
  /// </summary>
  public List<MemberFeatureDto> Features { get; init; } = new();

  /// <summary>
  /// Member eligibility summary
  /// </summary>
  public MemberEligibilitySummaryDto EligibilitySummary { get; init; } = null!;

  /// <summary>
  /// Member request statistics
  /// </summary>
  public MemberRequestStatisticsDto RequestStatistics { get; init; } = null!;

  /// <summary>
  /// Member profile completeness
  /// </summary>
  public MemberProfileCompletenessDto ProfileCompleteness { get; init; } = null!;

  /// <summary>
  /// Member metadata
  /// </summary>
  public Dictionary<string, string> Metadata { get; init; } = new();

  /// <summary>
  /// Last login date
  /// </summary>
  public DateTime? LastLoginDate { get; init; }

  /// <summary>
  /// Last modification timestamp
  /// </summary>
  public DateTime? LastModifiedAt { get; init; }

  /// <summary>
  /// Member preferences
  /// </summary>
  public Dictionary<string, object> Preferences { get; init; } = new();
}
