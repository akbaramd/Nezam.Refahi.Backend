using MediatR;
using Nezam.Refahi.Facilities.Domain.Enums;
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
    /// Rejection type
    /// </summary>
    public FacilityRejectionType RejectionType { get; init; } = FacilityRejectionType.General;

    /// <summary>
    /// Additional rejection details
    /// </summary>
    public string? Details { get; init; }

    /// <summary>
    /// Additional notes
    /// </summary>
    public string? Notes { get; init; }

    /// <summary>
    /// Rejector user ID
    /// </summary>
    public Guid RejectorUserId { get; init; }

    /// <summary>
    /// Rejector user name
    /// </summary>
    public string? RejectorUserName { get; init; }
}