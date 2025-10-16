namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilities;

/// <summary>
/// Response for facilities list
/// </summary>
public record GetFacilitiesResult
{
  /// <summary>
  /// List of facilities
  /// </summary>
  public List<FacilityDto> Facilities { get; init; } = new();

  /// <summary>
  /// Total count of facilities
  /// </summary>
  public int TotalCount { get; init; }

  /// <summary>
  /// Current page number
  /// </summary>
  public int Page { get; init; }

  /// <summary>
  /// Page size
  /// </summary>
  public int PageSize { get; init; }

  /// <summary>
  /// Total pages
  /// </summary>
  public int TotalPages { get; init; }
}