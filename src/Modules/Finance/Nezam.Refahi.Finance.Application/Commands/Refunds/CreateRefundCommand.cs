using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Commands.Refunds;

/// <summary>
/// Command to create a refund request for a bill
/// </summary>
public record CreateRefundCommand : IRequest<ApplicationResult<CreateRefundResponse>>
{
    /// <summary>
    /// ID of the bill to create refund for
    /// </summary>
    public Guid BillId { get; init; }

    /// <summary>
    /// Refund amount in Rials
    /// </summary>
    public decimal RefundAmountRials { get; init; }

    /// <summary>
    /// Reason for the refund
    /// </summary>
    public string Reason { get; init; } = string.Empty;

    /// <summary>
    /// National number of the person requesting the refund (optional, defaults to bill owner)
    /// </summary>
    public Guid? RequestedByExternalUserId { get; init; }
}

/// <summary>
/// Response for CreateRefundCommand
/// </summary>
public record CreateRefundResponse
{
    public Guid RefundId { get; init; }
    public Guid BillId { get; init; }
    public decimal RefundAmount { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime RequestedAt { get; init; }
}
