namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityDetails;

/// <summary>
/// Response for facility details
/// </summary>
public record GetFacilityDetailsResult
{
  /// <summary>
  /// Facility details
  /// </summary>
  public FacilityDetailsDto Facility { get; init; } = null!;
}