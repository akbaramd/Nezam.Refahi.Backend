using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Commands.Payments;

/// <summary>
/// Command to pay a bill using wallet balance
/// </summary>
public record PayWithWalletCommand : IRequest<ApplicationResult<PayWithWalletResponse>>
{
    /// <summary>
    /// ID of the payment to complete
    /// </summary>
    public Guid PaymentId { get; init; }

    /// <summary>
    /// ID of the bill to pay
    /// </summary>
    public Guid BillId { get; init; }

    /// <summary>
    /// ID of the wallet to use for payment
    /// </summary>
    public Guid WalletId { get; init; }

    /// <summary>
    /// Payment amount in Rials (if not specified, will pay the remaining bill amount)
    /// </summary>
    public decimal? AmountRials { get; init; }

    /// <summary>
    /// Description for the payment
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// External reference for tracking
    /// </summary>
    public string? ExternalReference { get; init; }

    /// <summary>
    /// Additional metadata
    /// </summary>
    public Dictionary<string, string>? Metadata { get; init; }
}

/// <summary>
/// Response for PayWithWalletCommand
/// </summary>
public record PayWithWalletResponse
{
    public Guid PaymentId { get; init; }
    public Guid BillId { get; init; }
    public string BillNumber { get; init; } = string.Empty;
    public decimal AmountPaidRials { get; init; }
    public string PaymentStatus { get; init; } = string.Empty;
    public string PaymentStatusText { get; init; } = string.Empty; // Persian status text
    public string BillStatus { get; init; } = string.Empty;
    public string BillStatusText { get; init; } = string.Empty; // Persian status text
    public decimal BillRemainingAmountRials { get; init; }
    public decimal WalletBalanceAfterPaymentRials { get; init; }
    public DateTime ProcessedAt { get; init; }
    public string? Description { get; init; }
    public string? ExternalReference { get; init; }
    public Guid WalletTransactionId { get; init; }
    public Dictionary<string, string> Metadata { get; init; } = new();
}
