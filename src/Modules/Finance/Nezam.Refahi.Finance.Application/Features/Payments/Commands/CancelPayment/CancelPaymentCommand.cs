using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Commands.Payments;

/// <summary>
/// Command to cancel a payment
/// </summary>
public record CancelPaymentCommand : IRequest<ApplicationResult<CancelPaymentResponse>>
{
    /// <summary>
    /// ID of the payment to cancel
    /// </summary>
    public Guid PaymentId { get; init; }

    /// <summary>
    /// Reason for cancellation (optional)
    /// </summary>
    public string? Reason { get; init; }
}

/// <summary>
/// Response for CancelPaymentCommand
/// </summary>
public record CancelPaymentResponse
{
    public Guid PaymentId { get; init; }
    public Guid BillId { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? CancellationReason { get; init; }
}
