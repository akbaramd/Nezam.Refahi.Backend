using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityRequests;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserMemberInfo;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserCycleRequests;

/// <summary>
/// Response for user cycle requests query
/// </summary>
public record GetUserCycleRequestsResponse
{
  /// <summary>
  /// User member information
  /// </summary>
  public UserMemberInfoDto UserInfo { get; init; } = null!;

  /// <summary>
  /// List of user's cycle requests
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

  /// <summary>
  /// Request statistics summary
  /// </summary>
  public UserRequestStatisticsDto Statistics { get; init; } = null!;
}