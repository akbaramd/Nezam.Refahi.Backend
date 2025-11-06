namespace Nezam.Refahi.Facilities.Application.Dtos;

/// <summary>
/// Detailed facility cycle data transfer object
/// Contains full cycle information including all details
/// </summary>
public class FacilityCycleWithUserDetailDto : FacilityCycleWithUserDto
{


  /// <summary>
  /// Cycle dependencies
  /// </summary>
  public List<FacilityCycleDependencyDto> Dependencies { get; init; } = new();

  /// <summary>
  /// Cycle metadata
  /// </summary>
  public Dictionary<string, string> Metadata { get; init; } = new();

  /// <summary>
  /// Last modification timestamp
  /// </summary>
  public DateTime? LastModifiedAt { get; init; }

  /// <summary>
  /// Cycle statistics
  /// </summary>
  public CycleStatisticsDto Statistics { get; init; } = null!;

  /// <summary>

  /// <summary>
  /// User request history for this cycle (if user context provided)
  /// </summary>
  public List<UserRequestHistoryDto> UserRequestHistory { get; init; } = new();
}