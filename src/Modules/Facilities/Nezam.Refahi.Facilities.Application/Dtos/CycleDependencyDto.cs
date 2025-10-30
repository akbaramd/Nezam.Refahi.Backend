namespace Nezam.Refahi.Facilities.Application.Dtos;

/// <summary>
/// Cycle dependency data transfer object
/// </summary>
public record CycleDependencyDto
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
  /// Indicates if facility must be completed
  /// </summary>
  public bool MustBeCompleted { get; init; }

  /// <summary>
  /// Dependency creation date
  /// </summary>
  public DateTime CreatedAt { get; init; }

  /// <summary>
  /// Indicates if user satisfies this dependency
  /// </summary>
  public bool IsSatisfied { get; init; }

  /// <summary>
  /// User's request status for required facility
  /// </summary>
  public string? UserRequestStatus { get; init; }
}