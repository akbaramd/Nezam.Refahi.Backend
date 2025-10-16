using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;

/// <summary>
/// Query to get detailed information about a specific facility cycle with user context
/// </summary>
public record GetFacilityCycleDetailsQuery : IRequest<ApplicationResult<GetFacilityCycleDetailsResponse>>
{
    /// <summary>
    /// Facility cycle ID
    /// </summary>
    public Guid CycleId { get; init; }

    /// <summary>
    /// User national number for eligibility checking and request status
    /// </summary>
    public string? NationalNumber { get; init; }

    /// <summary>
    /// Include detailed eligibility analysis
    /// </summary>
    public bool IncludeEligibilityDetails { get; init; } = true;

    /// <summary>
    /// Include user request history for this cycle
    /// </summary>
    public bool IncludeUserRequestHistory { get; init; } = true;

    /// <summary>
    /// Include cycle dependencies
    /// </summary>
    public bool IncludeDependencies { get; init; } = true;

    /// <summary>
    /// Include facility information
    /// </summary>
    public bool IncludeFacilityInfo { get; init; } = true;

    /// <summary>
    /// Include cycle statistics
    /// </summary>
    public bool IncludeStatistics { get; init; } = true;
}