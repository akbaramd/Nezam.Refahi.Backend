using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Commands.Payments;

/// <summary>
/// Command to create a payment for a bill with optional bill item additions
/// </summary>
public record CreatePaymentCommand : IRequest<ApplicationResult<CreatePaymentResponse>>
{
    /// <summary>
    /// ID of the bill to create payment for
    /// </summary>
    public Guid BillId { get; init; }

    /// <summary>
    /// External user ID requesting the payment
    /// </summary>
    public Guid ExternalUserId { get; init; }

    /// <summary>
    /// Payment amount in Rials (if not specified, will be calculated from bill total)
    /// </summary>
    public decimal AmountRials { get; init; }

    /// <summary>
    /// Payment method (Online, Cash, Card)
    /// </summary>
    public string PaymentMethod { get; init; } = "Online";

    /// <summary>
    /// Payment gateway (Zarinpal, Mellat, etc.)
    /// </summary>
    public string? PaymentGateway { get; init; }

    /// <summary>
    /// Callback URL for online payments
    /// </summary>
    public string? CallbackUrl { get; init; }

    /// <summary>
    /// Description for the payment
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Expiry date for the payment
    /// </summary>
    public DateTime? ExpiryDate { get; init; }



    /// <summary>
    /// Whether to automatically issue the bill if it's in draft status
    /// </summary>
    public bool AutoIssueBill { get; init; } = true;

    /// <summary>
    /// Discount code to apply before payment (optional)
    /// </summary>
    public string? DiscountCode { get; init; }

    /// <summary>
    /// Whether to apply discount even if it exceeds bill amount (mabaghi misuzad)
    /// </summary>
    public bool AllowOverDiscount { get; init; } = true;

    /// <summary>
    /// Whether to skip payment if final amount becomes zero (free payment)
    /// </summary>
    public bool SkipPaymentIfZero { get; init; } = true;
}

/// <summary>
/// Bill item to be added during payment creation
/// </summary>
public record CreatePaymentBillItem
{
    /// <summary>
    /// Title of the bill item
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Description of the item (optional)
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Unit price in Rials
    /// </summary>
    public decimal UnitPriceRials { get; init; }

    /// <summary>
    /// Quantity of the item
    /// </summary>
    public int Quantity { get; init; } = 1;

    /// <summary>
    /// Discount percentage (optional, 0-100)
    /// </summary>
    public decimal? DiscountPercentage { get; init; }
}

/// <summary>
/// Response for CreatePaymentCommand
/// </summary>
public record CreatePaymentResponse
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
    public int ItemsAdded { get; init; }
    public bool BillWasIssued { get; init; }
    
    // Payment processing fields
    public long? TrackingNumber { get; init; }
    public bool RequiresRedirect { get; init; }
    public string? PaymentMessage { get; init; }
    public string? PaymentGateway { get; init; }
    
    // Discount information
    public string? AppliedDiscountCode { get; init; }
    public decimal AppliedDiscountAmount { get; init; }
    public decimal OriginalBillAmount { get; init; }
    public decimal FinalBillAmount { get; init; }
    public bool IsFreePayment { get; init; }
    public bool PaymentSkipped { get; init; }
    public string PaymentStatus { get; init; } = string.Empty;
}
