using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Commands.CancelFacilityRequest;

/// <summary>
/// Command to cancel a facility request
/// </summary>
public record CancelFacilityRequestCommand : IRequest<ApplicationResult<CancelFacilityRequestResult>>
{
    /// <summary>
    /// Facility request ID to cancel
    /// </summary>
    public Guid RequestId { get; init; }

    /// <summary>
    /// Cancellation reason (optional)
    /// </summary>
    public string? Reason { get; init; }

    /// <summary>
    /// User ID who is cancelling (usually the requester)
    /// </summary>
    public Guid CancelledByUserId { get; init; }
}