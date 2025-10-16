using MediatR;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Application.Features.Payments.Commands.HandleCallback;

/// <summary>
/// Command to handle payment callback from gateway
/// </summary>
public record HandlePaymentCallbackCommand : IRequest<ApplicationResult<PaymentCallbackResult>>
{
    // No parameters needed as Parbad will handle the callback data from HTTP context
}

/// <summary>
/// Result of payment callback processing
/// Contains both payment and comprehensive bill information
/// </summary>
public class PaymentCallbackResult
{
    // Payment Information
    public Guid PaymentId { get; set; }
    public decimal TrackingNumber { get; set; }
    public bool IsSuccessful { get; set; }
    public string? Message { get; set; }
    public string? TransactionCode { get; set; }
    public Money Amount { get; set; } = null!;
    public DateTime ProcessedAt { get; set; }
    public string? RedirectUrl { get; set; }
    public PaymentStatus NewStatus { get; set; }
    
    // Bill Information
    public Guid BillId { get; set; }
    public string? BillNumber { get; set; }
    public string? BillStatus { get; set; }
    public decimal? BillTotalAmount { get; set; }
    public decimal? BillPaidAmount { get; set; }
    public decimal? BillRemainingAmount { get; set; }
    public bool IsBillFullyPaid { get; set; }
    public DateTime? BillFullyPaidDate { get; set; }
}
