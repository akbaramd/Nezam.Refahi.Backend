using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Commands.Payments;

/// <summary>
/// Command to complete a payment (mark as successful)
/// </summary>
public record CompletePaymentCommand : IRequest<ApplicationResult<CompletePaymentResponse>>
{
    /// <summary>
    /// ID of the payment to complete
    /// </summary>
    public Guid PaymentId { get; init; }

    /// <summary>
    /// Gateway transaction ID from payment provider
    /// </summary>
    public string GatewayTransactionId { get; init; } = string.Empty;

    /// <summary>
    /// Gateway reference number (optional)
    /// </summary>
    public string? GatewayReference { get; init; }
}

/// <summary>
/// Response for CompletePaymentCommand
/// </summary>
public record CompletePaymentResponse
{
    public Guid PaymentId { get; init; }
    public Guid BillId { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CompletedAt { get; init; }
    public string BillStatus { get; init; } = string.Empty;
    public decimal BillRemainingAmount { get; init; }
}
