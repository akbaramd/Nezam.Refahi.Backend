namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityRequestDetails;

/// <summary>
/// Response for facility request details
/// </summary>
public record GetFacilityRequestDetailsResult
{
  /// <summary>
  /// Facility request details
  /// </summary>
  public FacilityRequestDetailsDto Request { get; init; } = null!;
}