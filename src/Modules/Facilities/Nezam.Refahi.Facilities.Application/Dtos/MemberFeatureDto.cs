namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserMemberInfo;

/// <summary>
/// Member feature data transfer object
/// </summary>
public record MemberFeatureDto
{
  /// <summary>
  /// Feature ID
  /// </summary>
  public string FeatureId { get; init; } = null!;

  /// <summary>
  /// Feature name
  /// </summary>
  public string FeatureName { get; init; } = null!;

  /// <summary>
  /// Feature description
  /// </summary>
  public string? Description { get; init; }

  /// <summary>
  /// Feature category
  /// </summary>
  public string? Category { get; init; }

  /// <summary>
  /// Feature assignment date
  /// </summary>
  public DateTime AssignedAt { get; init; }

  /// <summary>
  /// Feature expiration date (if applicable)
  /// </summary>
  public DateTime? ExpiresAt { get; init; }

  /// <summary>
  /// Indicates if feature is active
  /// </summary>
  public bool IsActive { get; init; }
}