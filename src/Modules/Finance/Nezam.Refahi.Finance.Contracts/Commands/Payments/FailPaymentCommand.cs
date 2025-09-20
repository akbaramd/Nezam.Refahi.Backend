using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Contracts.Commands.Payments;

/// <summary>
/// Command to mark a payment as failed
/// </summary>
public record FailPaymentCommand : IRequest<ApplicationResult<FailPaymentResponse>>
{
    /// <summary>
    /// ID of the payment to mark as failed
    /// </summary>
    public Guid PaymentId { get; init; }

    /// <summary>
    /// Reason for payment failure
    /// </summary>
    public string? FailureReason { get; init; }
}

/// <summary>
/// Response for FailPaymentCommand
/// </summary>
public record FailPaymentResponse
{
    public Guid PaymentId { get; init; }
    public Guid BillId { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? FailureReason { get; init; }
}