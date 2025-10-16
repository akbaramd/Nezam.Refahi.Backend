namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;

/// <summary>
/// Facility information data transfer object
/// </summary>
public record FacilityInfoDto
{
  /// <summary>
  /// Facility ID
  /// </summary>
  public Guid Id { get; init; }

  /// <summary>
  /// Facility name
  /// </summary>
  public string Name { get; init; } = null!;

  /// <summary>
  /// Facility code
  /// </summary>
  public string Code { get; init; } = null!;

  /// <summary>
  /// Facility type
  /// </summary>
  public string Type { get; init; } = null!;

  /// <summary>
  /// Human-readable facility type text
  /// </summary>
  public string TypeText { get; init; } = null!;

  /// <summary>
  /// Facility status
  /// </summary>
  public string Status { get; init; } = null!;

  /// <summary>
  /// Human-readable status text
  /// </summary>
  public string StatusText { get; init; } = null!;

  /// <summary>
  /// Facility description
  /// </summary>
  public string? Description { get; init; }

  /// <summary>
  /// Bank information
  /// </summary>
  public BankInfoDto? BankInfo { get; init; }

  /// <summary>
  /// Indicates if facility is active
  /// </summary>
  public bool IsActive { get; init; }
}