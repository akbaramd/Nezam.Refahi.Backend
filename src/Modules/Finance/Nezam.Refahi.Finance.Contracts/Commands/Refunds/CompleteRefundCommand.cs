using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Contracts.Commands.Refunds;

/// <summary>
/// Command to complete a refund (mark as successful)
/// </summary>
public record CompleteRefundCommand : IRequest<ApplicationResult<CompleteRefundResponse>>
{
    /// <summary>
    /// ID of the refund to complete
    /// </summary>
    public Guid RefundId { get; init; }

    /// <summary>
    /// Gateway refund ID from payment provider (optional)
    /// </summary>
    public string? GatewayRefundId { get; init; }

    /// <summary>
    /// Gateway reference number (optional)
    /// </summary>
    public string? GatewayReference { get; init; }
}

/// <summary>
/// Response for CompleteRefundCommand
/// </summary>
public record CompleteRefundResponse
{
    public Guid RefundId { get; init; }
    public Guid BillId { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CompletedAt { get; init; }
    public string BillStatus { get; init; } = string.Empty;
    public decimal BillRemainingAmount { get; init; }
}