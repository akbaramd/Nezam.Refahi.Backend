using MediatR;
using Nezam.Refahi.Facilities.Application.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityDetails;

/// <summary>
/// Query to get facility details by ID
/// </summary>
public record GetFacilityDetailsQuery : IRequest<ApplicationResult<FacilityDetailsDto>>
{
    /// <summary>
    /// Facility ID
    /// </summary>
    public Guid FacilityId { get; init; }

    /// <summary>
    /// Include active cycles
    /// </summary>
    public bool IncludeCycles { get; init; } = true;

    /// <summary>
    /// Include features
    /// </summary>
    public bool IncludeFeatures { get; init; } = true;

    /// <summary>
    /// Include capability policies
    /// </summary>
    public bool IncludePolicies { get; init; } = true;

    /// <summary>
    /// User national number for user context (optional)
    /// </summary>
    public string? NationalNumber { get; init; }

    /// <summary>
    /// Include user request history for cycles
    /// </summary>
    public bool IncludeUserRequestHistory { get; init; } = false;
}