using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserCycleRequests;

/// <summary>
/// Query to get user's cycle requests across all facilities
/// </summary>
public record GetUserCycleRequestsQuery : IRequest<ApplicationResult<GetUserCycleRequestsResponse>>
{
    /// <summary>
    /// User national number for identification
    /// </summary>
    public string NationalNumber { get; init; } = null!;

    /// <summary>
    /// Page number (default: 1)
    /// </summary>
    public int Page { get; init; } = 1;

    /// <summary>
    /// Page size (default: 10)
    /// </summary>
    public int PageSize { get; init; } = 10;

    /// <summary>
    /// Filter by facility ID
    /// </summary>
    public Guid? FacilityId { get; init; }

    /// <summary>
    /// Filter by facility cycle ID
    /// </summary>
    public Guid? FacilityCycleId { get; init; }

    /// <summary>
    /// Filter by request status
    /// </summary>
    public string? Status { get; init; }

    /// <summary>
    /// Filter by request status category
    /// </summary>
    public RequestStatusCategory? StatusCategory { get; init; }

    /// <summary>
    /// Date from filter
    /// </summary>
    public DateTime? DateFrom { get; init; }

    /// <summary>
    /// Date to filter
    /// </summary>
    public DateTime? DateTo { get; init; }

    /// <summary>
    /// Include only active requests
    /// </summary>
    public bool OnlyActive { get; init; } = false;

    /// <summary>
    /// Include cycle information
    /// </summary>
    public bool IncludeCycleInfo { get; init; } = true;

    /// <summary>
    /// Include facility information
    /// </summary>
    public bool IncludeFacilityInfo { get; init; } = true;

    /// <summary>
    /// Include request timeline
    /// </summary>
    public bool IncludeTimeline { get; init; } = true;

    /// <summary>
    /// Include request statistics
    /// </summary>
    public bool IncludeStatistics { get; init; } = true;
}