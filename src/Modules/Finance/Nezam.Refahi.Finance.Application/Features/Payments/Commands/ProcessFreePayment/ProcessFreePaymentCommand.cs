using MediatR;
using Nezam.Refahi.Finance.Application.Commands.Payments;
using Nezam.Refahi.Finance.Application.Features.Payments.Commands.CompletePayment;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Commands.Payments;

/// <summary>
/// Command to process free payments (zero-amount bills or 100% discount)
/// </summary>
public record ProcessFreePaymentCommand : IRequest<ApplicationResult<ProcessFreePaymentResponse>>
{
    public Guid BillId { get; init; }
    public Guid ExternalUserId { get; init; }
    public string? Description { get; init; }
}

/// <summary>
/// Response for free payment processing
/// </summary>
public record ProcessFreePaymentResponse
{
    public Guid PaymentId { get; init; }
    public Guid BillId { get; init; }
    public string BillNumber { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string PaymentMethod { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public string BillStatus { get; init; } = string.Empty;
    public decimal BillTotalAmount { get; init; }
    public bool IsFreePayment { get; init; }
    public string PaymentStatus { get; init; } = string.Empty;
}
