namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityRequests;

/// <summary>
/// Response for facility requests list
/// </summary>
public record GetFacilityRequestsResult
{
  /// <summary>
  /// List of facility requests
  /// </summary>
  public List<FacilityRequestDto> Requests { get; init; } = new();

  /// <summary>
  /// Total count of requests
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