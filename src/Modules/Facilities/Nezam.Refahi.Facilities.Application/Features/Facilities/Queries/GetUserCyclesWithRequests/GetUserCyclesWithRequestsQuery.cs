using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserCyclesWithRequests;

/// <summary>
/// Query to get cycles where the current user has requests with detailed request information
/// </summary>
public record GetUserCyclesWithRequestsQuery : IRequest<ApplicationResult<GetUserCyclesWithRequestsResponse>>
{
    /// <summary>
    /// User national number (required)
    /// </summary>
    public string NationalNumber { get; init; } = null!;

    /// <summary>
    /// Facility ID to filter cycles (optional)
    /// </summary>
    public Guid? FacilityId { get; init; }

    /// <summary>
    /// Page number (default: 1)
    /// </summary>
    public int Page { get; init; } = 1;

    /// <summary>
    /// Page size (default: 10)
    /// </summary>
    public int PageSize { get; init; } = 10;

    /// <summary>
    /// Filter by request status
    /// </summary>
    public string? RequestStatus { get; init; }

    /// <summary>
    /// Filter by request status category
    /// </summary>
    public RequestStatusCategory? RequestStatusCategory { get; init; }

    /// <summary>
    /// Include only active cycles
    /// </summary>
    public bool OnlyActive { get; init; } = true;

    /// <summary>
    /// Include detailed request information
    /// </summary>
    public bool IncludeDetailedRequestInfo { get; init; } = true;

    /// <summary>
    /// Include facility information
    /// </summary>
    public bool IncludeFacilityInfo { get; init; } = true;

    /// <summary>
    /// Include cycle statistics
    /// </summary>
    public bool IncludeStatistics { get; init; } = true;
}

/// <summary>
/// Request status categories for filtering
/// </summary>
public enum RequestStatusCategory
{
    /// <summary>
    /// All statuses
    /// </summary>
    All,

    /// <summary>
    /// In progress statuses
    /// </summary>
    InProgress,

    /// <summary>
    /// Completed statuses
    /// </summary>
    Completed,

    /// <summary>
    /// Rejected statuses
    /// </summary>
    Rejected,

    /// <summary>
    /// Cancelled statuses
    /// </summary>
    Cancelled,

    /// <summary>
    /// Terminal statuses (final states)
    /// </summary>
    Terminal
}
