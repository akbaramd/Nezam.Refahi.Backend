using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;
using System.Text.Json.Serialization;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Commands.CreateFacilityRequest;

/// <summary>
/// Command to create a new facility request
/// </summary>
public record CreateFacilityRequestCommand : IRequest<ApplicationResult<CreateFacilityRequestResult>>
{
    /// <summary>
    /// Facility cycle ID to request
    /// </summary>
    public Guid FacilityCycleId { get; init; }

    /// <summary>
    /// Selected price option ID from cycle's price options
    /// </summary>
    public Guid PriceOptionId { get; init; }

    /// <summary>
    /// Request description (optional)
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Additional metadata (optional)
    /// </summary>
    public Dictionary<string, string>? Metadata { get; init; }

    /// <summary>
    /// Idempotency key for duplicate request prevention
    /// </summary>
    public string? IdempotencyKey { get; init; }

    /// <summary>
    /// National number for member identification (set by endpoint, ignored from request body)
    /// </summary>
    [JsonIgnore]
    public string? NationalNumber { get; init; }
}