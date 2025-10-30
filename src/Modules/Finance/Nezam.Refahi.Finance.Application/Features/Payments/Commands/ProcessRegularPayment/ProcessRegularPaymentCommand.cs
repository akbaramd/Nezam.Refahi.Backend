using MediatR;
using Nezam.Refahi.Finance.Application.Commands.Payments;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Commands.Payments;

/// <summary>
/// Command to process regular payments (non-zero amount bills)
/// </summary>
public record ProcessRegularPaymentCommand : IRequest<ApplicationResult<ProcessRegularPaymentResponse>>
{
    public Guid BillId { get; init; }
    public decimal AmountRials { get; init; }
    public string PaymentMethod { get; init; } = string.Empty;
    public string? PaymentGateway { get; init; }
    public string? CallbackUrl { get; init; }
    public string? Description { get; init; }
    public DateTime? ExpiryDate { get; init; }
    public Guid ExternalUserId { get; init; }
}

/// <summary>
/// Response for regular payment processing
/// </summary>
public record ProcessRegularPaymentResponse
{
    public Guid PaymentId { get; init; }
    public Guid BillId { get; init; }
    public string BillNumber { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string PaymentMethod { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? ExpiryDate { get; init; }
    public string? GatewayRedirectUrl { get; init; }
    public string BillStatus { get; init; } = string.Empty;
    public decimal BillTotalAmount { get; init; }
    public long? TrackingNumber { get; init; }
    public bool RequiresRedirect { get; init; }
    public string? PaymentMessage { get; init; }
    public string? PaymentGateway { get; init; }
}
