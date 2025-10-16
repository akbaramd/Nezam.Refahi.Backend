namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;

/// <summary>
/// Cooldown requirements analysis data transfer object
/// </summary>
public record CooldownRequirementsDto
{
  /// <summary>
  /// Indicates if user meets cooldown requirements
  /// </summary>
  public bool MeetsRequirements { get; init; }

  /// <summary>
  /// Required cooldown days
  /// </summary>
  public int RequiredCooldownDays { get; init; }

  /// <summary>
  /// Days since last facility received
  /// </summary>
  public int? DaysSinceLastFacility { get; init; }

  /// <summary>
  /// Last facility received date
  /// </summary>
  public DateTime? LastFacilityReceivedAt { get; init; }

  /// <summary>
  /// Days remaining in cooldown
  /// </summary>
  public int? DaysRemainingInCooldown { get; init; }
}