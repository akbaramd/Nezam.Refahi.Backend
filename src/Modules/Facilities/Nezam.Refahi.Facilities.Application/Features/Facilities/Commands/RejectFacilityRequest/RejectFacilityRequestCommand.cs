using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Commands.RejectFacilityRequest;

/// <summary>
/// Command to reject a facility request
/// </summary>
public record RejectFacilityRequestCommand : IRequest<ApplicationResult<RejectFacilityRequestResult>>
{
    /// <summary>
    /// Facility request ID to reject
    /// </summary>
    public Guid RequestId { get; init; }

    /// <summary>
    /// Rejection reason (required)
    /// </summary>
    public string Reason { get; init; } = null!;

    /// <summary>
    /// Rejector user ID
    /// </summary>
    public Guid RejectorUserId { get; init; }
}