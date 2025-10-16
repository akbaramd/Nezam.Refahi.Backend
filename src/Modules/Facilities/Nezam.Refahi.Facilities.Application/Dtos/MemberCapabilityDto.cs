namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserMemberInfo;

/// <summary>
/// Member capability data transfer object
/// </summary>
public record MemberCapabilityDto
{
  /// <summary>
  /// Capability ID
  /// </summary>
  public string CapabilityId { get; init; } = null!;

  /// <summary>
  /// Capability name
  /// </summary>
  public string CapabilityName { get; init; } = null!;

  /// <summary>
  /// Capability description
  /// </summary>
  public string? Description { get; init; }

  /// <summary>
  /// Capability category
  /// </summary>
  public string? Category { get; init; }

  /// <summary>
  /// Capability level
  /// </summary>
  public string? Level { get; init; }

  /// <summary>
  /// Capability assignment date
  /// </summary>
  public DateTime AssignedAt { get; init; }

  /// <summary>
  /// Capability expiration date (if applicable)
  /// </summary>
  public DateTime? ExpiresAt { get; init; }

  /// <summary>
  /// Indicates if capability is active
  /// </summary>
  public bool IsActive { get; init; }
}