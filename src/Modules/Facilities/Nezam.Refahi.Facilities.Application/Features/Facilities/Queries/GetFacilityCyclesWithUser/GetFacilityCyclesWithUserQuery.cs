using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycles;

/// <summary>
/// Query to get cycles for a specific facility with user context
/// </summary>
public record GetFacilityCyclesQuery : IRequest<ApplicationResult<GetFacilityCyclesResponse>>
{
    /// <summary>
    /// Facility ID to get cycles for
    /// </summary>
    public Guid FacilityId { get; init; }

    /// <summary>
    /// User national number for eligibility checking
    /// </summary>
    public string? NationalNumber { get; init; }

    /// <summary>
    /// Page number (default: 1)
    /// </summary>
    public int Page { get; init; } = 1;

    /// <summary>
    /// Page size (default: 10)
    /// </summary>
    public int PageSize { get; init; } = 10;

    /// <summary>
    /// Filter by cycle status
    /// </summary>
    public string? Status { get; init; }

    /// <summary>
    /// Free-text search term (name/description)
    /// </summary>
    public string? SearchTerm { get; init; }

    /// <summary>
    /// Include only active cycles
    /// </summary>
    public bool OnlyActive { get; init; } = true;

    /// <summary>
    /// Include only cycles user is eligible for
    /// </summary>
    public bool OnlyEligible { get; init; } = false;

    /// <summary>
    /// Include only cycles where user has requests
    /// </summary>
    public bool OnlyWithUserRequests { get; init; } = false;

    /// <summary>
    /// Include user's request status for each cycle
    /// </summary>
    public bool IncludeUserRequestStatus { get; init; } = true;

    /// <summary>
    /// Include detailed request information for each cycle
    /// </summary>
    public bool IncludeDetailedRequestInfo { get; init; } = false;

    /// <summary>
    /// Include cycle statistics
    /// </summary>
    public bool IncludeStatistics { get; init; } = true;

}