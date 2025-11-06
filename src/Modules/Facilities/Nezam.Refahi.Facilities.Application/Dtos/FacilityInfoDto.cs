namespace Nezam.Refahi.Facilities.Application.Dtos;

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
  /// Facility description
  /// </summary>
  public string? Description { get; init; }

  /// <summary>
  /// Bank information
  /// </summary>
  public BankInfoDto? BankInfo { get; init; }
}