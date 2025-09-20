using MediatR;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.Tours.Queries.GetTourDetail;

/// <summary>
/// Query to get detailed information about a specific tour
/// </summary>
public record GetTourDetailQuery : IRequest<ApplicationResult<TourDetailDto>>
{
    /// <summary>
    /// Tour ID to get details for
    /// </summary>
    public Guid TourId { get; init; }

    /// <summary>
    /// Whether to include user-specific information (requires authentication)
    /// </summary>
    public bool IncludeUserInfo { get; init; } = true;

    /// <summary>
    /// Whether to include statistics and analytics data
    /// </summary>
    public bool IncludeStatistics { get; init; } = true;

    /// <summary>
    /// Whether to include capacity details
    /// </summary>
    public bool IncludeCapacityDetails { get; init; } = true;

    /// <summary>
    /// Whether to include pricing information
    /// </summary>
    public bool IncludePricing { get; init; } = true;

    /// <summary>
    /// Whether to include photos and media
    /// </summary>
    public bool IncludeMedia { get; init; } = true;

    /// <summary>
    /// Whether to include features and amenities
    /// </summary>
    public bool IncludeFeatures { get; init; } = true;

    /// <summary>
    /// Whether to include restricted tours
    /// </summary>
    public bool IncludeRestrictions { get; init; } = true;
}
