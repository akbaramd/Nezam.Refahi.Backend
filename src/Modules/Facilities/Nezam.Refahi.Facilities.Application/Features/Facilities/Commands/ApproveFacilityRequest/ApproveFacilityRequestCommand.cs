using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Commands.ApproveFacilityRequest;

/// <summary>
/// Command to approve a facility request
/// </summary>
public record ApproveFacilityRequestCommand : IRequest<ApplicationResult<ApproveFacilityRequestResult>>
{
    /// <summary>
    /// Facility request ID to approve
    /// </summary>
    public Guid RequestId { get; init; }

    /// <summary>
    /// Approved amount in Rials
    /// </summary>
    public decimal ApprovedAmountRials { get; init; }

    /// <summary>
    /// Currency code (default: IRR)
    /// </summary>
    public string Currency { get; init; } = "IRR";

    /// <summary>
    /// Approval notes (optional)
    /// </summary>
    public string? Notes { get; init; }

    /// <summary>
    /// Approver user ID
    /// </summary>
    public Guid ApproverUserId { get; init; }
}