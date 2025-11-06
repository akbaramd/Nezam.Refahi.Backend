using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilities;

/// <summary>
/// Query to get list of facilities
/// </summary>
public record GetFacilitiesQuery : IRequest<ApplicationResult<GetFacilitiesResult>>
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
    /// Filter by facility type
    /// </summary>


    /// <summary>
    /// Search term for name or description
    /// </summary>
    public string? SearchTerm { get; init; }

    /// <summary>
    /// Include only active facilities
    /// </summary>
    public bool OnlyActive { get; init; } = true;
}