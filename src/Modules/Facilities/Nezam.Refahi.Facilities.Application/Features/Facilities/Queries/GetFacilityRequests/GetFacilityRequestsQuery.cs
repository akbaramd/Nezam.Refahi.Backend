using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityRequests;

/// <summary>
/// Query to get facility requests
/// </summary>
public record GetFacilityRequestsQuery : IRequest<ApplicationResult<GetFacilityRequestsResult>>
{
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
    /// Filter by national number for member identification
    /// </summary>
    public string? NationalNumber { get; init; }

    /// <summary>
    /// Filter by request status
    /// </summary>
    public string? Status { get; init; }

    /// <summary>
    /// Search term for request number or user name
    /// </summary>
    public string? SearchTerm { get; init; }

    /// <summary>
    /// Date from filter
    /// </summary>
    public DateTime? DateFrom { get; init; }

    /// <summary>
    /// Date to filter
    /// </summary>
    public DateTime? DateTo { get; init; }
}