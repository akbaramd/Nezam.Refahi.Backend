using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserMemberInfo;

/// <summary>
/// Query to get user member information with facilities context
/// </summary>
public record GetUserMemberInfoQuery : IRequest<ApplicationResult<GetUserMemberInfoResponse>>
{
    /// <summary>
    /// User national number for identification
    /// </summary>
    public string NationalNumber { get; init; } = null!;

    /// <summary>
    /// Include member features
    /// </summary>
    public bool IncludeFeatures { get; init; } = true;

    /// <summary>
    /// Include member capabilities
    /// </summary>
    public bool IncludeCapabilities { get; init; } = true;

    /// <summary>
    /// Include member request statistics
    /// </summary>
    public bool IncludeRequestStatistics { get; init; } = true;

    /// <summary>
    /// Include member eligibility summary
    /// </summary>
    public bool IncludeEligibilitySummary { get; init; } = true;

    /// <summary>
    /// Include member profile completeness
    /// </summary>
    public bool IncludeProfileCompleteness { get; init; } = true;
}