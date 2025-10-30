namespace Nezam.Refahi.Facilities.Application.Dtos;

/// <summary>
/// Facility cycle dependency data transfer object
/// </summary>
public record FacilityCycleDependencyDto
{
  /// <summary>
  /// Dependency ID
  /// </summary>
  public Guid Id { get; init; }

  /// <summary>
  /// Required facility ID
  /// </summary>
  public Guid RequiredFacilityId { get; init; }

  /// <summary>
  /// Required facility name
  /// </summary>
  public string RequiredFacilityName { get; init; } = null!;

  /// <summary>
  /// Must be completed
  /// </summary>
  public bool MustBeCompleted { get; init; }

  /// <summary>
  /// Creation date
  /// </summary>
  public DateTime CreatedAt { get; init; }
}