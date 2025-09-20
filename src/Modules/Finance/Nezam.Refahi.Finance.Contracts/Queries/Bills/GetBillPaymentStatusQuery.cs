using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Contracts.Queries.Bills;

/// <summary>
/// Query to get bill payment status and related information
/// </summary>
public record GetBillPaymentStatusQuery : IRequest<ApplicationResult<BillPaymentStatusResponse>>
{
    /// <summary>
    /// Bill ID to check status for
    /// </summary>
    public Guid BillId { get; init; }

    /// <summary>
    /// Include payment history details
    /// </summary>
    public bool IncludePaymentHistory { get; init; } = false;

    /// <summary>
    /// Include refund history details
    /// </summary>
    public bool IncludeRefundHistory { get; init; } = false;

    /// <summary>
    /// Include bill items details
    /// </summary>
    public bool IncludeBillItems { get; init; } = false;
}

/// <summary>
/// Alternative query to get bill payment status by bill number
/// </summary>
public record GetBillPaymentStatusByNumberQuery : IRequest<ApplicationResult<BillPaymentStatusResponse>>
{
    /// <summary>
    /// Bill number to check status for
    /// </summary>
    public string BillNumber { get; init; } = string.Empty;

    /// <summary>
    /// Include payment history details
    /// </summary>
    public bool IncludePaymentHistory { get; init; } = false;

    /// <summary>
    /// Include refund history details
    /// </summary>
    public bool IncludeRefundHistory { get; init; } = false;

    /// <summary>
    /// Include bill items details
    /// </summary>
    public bool IncludeBillItems { get; init; } = false;
}

/// <summary>
/// Response for bill payment status queries
/// </summary>
public record BillPaymentStatusResponse
{
    public Guid BillId { get; init; }
    public string BillNumber { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string ReferenceId { get; init; } = string.Empty;
    public string BillType { get; init; } = string.Empty;
    public string UserNationalNumber { get; init; } = string.Empty;
    public string? UserFullName { get; init; }
    public string Status { get; init; } = string.Empty;
    public bool IsPaid { get; init; }
    public bool IsPartiallyPaid { get; init; }
    public bool IsOverdue { get; init; }
    public bool IsCancelled { get; init; }
    public decimal TotalAmountRials { get; init; }
    public decimal PaidAmountRials { get; init; }
    public decimal RemainingAmountRials { get; init; }
    public string? Description { get; init; }
    public DateTime IssueDate { get; init; }
    public DateTime? DueDate { get; init; }
    public DateTime? FullyPaidDate { get; init; }
    public Dictionary<string, string> Metadata { get; init; } = new();

    // Optional detailed information
    public List<BillPaymentHistoryDto>? PaymentHistory { get; init; }
    public List<BillRefundHistoryDto>? RefundHistory { get; init; }
    public List<BillItemSummaryDto>? BillItems { get; init; }

    // Calculated properties
    public decimal PaymentCompletionPercentage { get; init; }
    public int DaysUntilDue { get; init; }
    public int DaysOverdue { get; init; }
}

/// <summary>
/// Payment history item for bill status response
/// </summary>
public record BillPaymentHistoryDto
{
    public Guid PaymentId { get; init; }
    public decimal AmountRials { get; init; }
    public string Method { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string? Gateway { get; init; }
    public string? GatewayTransactionId { get; init; }
    public string? GatewayReference { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public string? FailureReason { get; init; }
}

/// <summary>
/// Refund history item for bill status response
/// </summary>
public record BillRefundHistoryDto
{
    public Guid RefundId { get; init; }
    public decimal AmountRials { get; init; }
    public string Reason { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string RequestedByNationalNumber { get; init; } = string.Empty;
    public DateTime RequestedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public string? GatewayRefundId { get; init; }
    public string? GatewayReference { get; init; }
}

/// <summary>
/// Bill item summary for bill status response
/// </summary>
public record BillItemSummaryDto
{
    public Guid ItemId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal UnitPriceRials { get; init; }
    public int Quantity { get; init; }
    public decimal? DiscountPercentage { get; init; }
    public decimal LineTotalRials { get; init; }
}