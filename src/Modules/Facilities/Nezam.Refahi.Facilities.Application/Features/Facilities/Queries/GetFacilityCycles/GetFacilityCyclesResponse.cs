using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;

using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserMemberInfo;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycles;

/// <summary>
/// Response for facility cycles query
/// </summary>
public record GetFacilityCyclesResponse
{
  /// <summary>
  /// Facility information
  /// </summary>
  public FacilityInfoDto Facility { get; init; } = null!;

  /// <summary>
  /// User member information (if NationalNumber provided)
  /// </summary>
  public UserMemberInfoDto? UserInfo { get; init; }

  /// <summary>
  /// List of cycles
  /// </summary>
  public List<FacilityCycleWithUserContextDto> Cycles { get; init; } = new();

  /// <summary>
  /// Total count of cycles
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
  /// Summary statistics
  /// </summary>
  public CycleSummaryDto Summary { get; init; } = null!;
}