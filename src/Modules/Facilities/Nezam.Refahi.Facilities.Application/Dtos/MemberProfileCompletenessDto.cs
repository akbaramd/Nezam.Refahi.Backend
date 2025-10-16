namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserMemberInfo;

/// <summary>
/// Member profile completeness data transfer object
/// </summary>
public record MemberProfileCompletenessDto
{
  /// <summary>
  /// Overall completeness percentage
  /// </summary>
  public decimal CompletenessPercentage { get; init; }

  /// <summary>
  /// Profile completeness status
  /// </summary>
  public ProfileCompletenessStatus Status { get; init; }

  /// <summary>
  /// Human-readable status description
  /// </summary>
  public string StatusDescription { get; init; } = null!;

  /// <summary>
  /// Profile completeness details
  /// </summary>
  public List<ProfileCompletenessItemDto> CompletenessItems { get; init; } = new();

  /// <summary>
  /// Missing required fields
  /// </summary>
  public List<string> MissingRequiredFields { get; init; } = new();

  /// <summary>
  /// Missing optional fields
  /// </summary>
  public List<string> MissingOptionalFields { get; init; } = new();

  /// <summary>
  /// Recommendations for profile improvement
  /// </summary>
  public List<string> Recommendations { get; init; } = new();
}