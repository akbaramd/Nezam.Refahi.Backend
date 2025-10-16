using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityRequestDetails;

/// <summary>
/// Query to get facility request details by ID
/// </summary>
public record GetFacilityRequestDetailsQuery : IRequest<ApplicationResult<GetFacilityRequestDetailsResult>>
{
    /// <summary>
    /// Facility request ID
    /// </summary>
    public Guid RequestId { get; init; }

    /// <summary>
    /// Include facility details
    /// </summary>
    public bool IncludeFacility { get; init; } = true;

    /// <summary>
    /// Include cycle details
    /// </summary>
    public bool IncludeCycle { get; init; } = true;

    /// <summary>
    /// Include policy snapshot
    /// </summary>
    public bool IncludePolicySnapshot { get; init; } = true;
}