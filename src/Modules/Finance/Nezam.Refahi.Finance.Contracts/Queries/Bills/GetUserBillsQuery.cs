using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Contracts.Queries.Bills;

/// <summary>
/// Query to get all bills for a specific user with payment status
/// </summary>
public record GetUserBillsQuery : IRequest<ApplicationResult<UserBillsResponse>>
{
    /// <summary>
    /// User's national number
    /// </summary>
    public Guid ExternalUserId { get; init; }

    /// <summary>
    /// Filter by bill status (optional)
    /// </summary>
    public string? Status { get; init; }

    /// <summary>
    /// Filter by bill type (optional)
    /// </summary>
    public string? BillType { get; init; }

    /// <summary>
    /// Include only overdue bills
    /// </summary>
    public bool OnlyOverdue { get; init; } = false;

    /// <summary>
    /// Include only unpaid bills
    /// </summary>
    public bool OnlyUnpaid { get; init; } = false;

    /// <summary>
    /// Page number for pagination (default: 1)
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Page size for pagination (default: 20, max: 100)
    /// </summary>
    public int PageSize { get; init; } = 20;

    /// <summary>
    /// Sort by field (IssueDate, DueDate, TotalAmount, Status)
    /// </summary>
    public string SortBy { get; init; } = "IssueDate";

    /// <summary>
    /// Sort direction (asc, desc)
    /// </summary>
    public string SortDirection { get; init; } = "desc";
}

/// <summary>
/// Response for GetUserBillsQuery
/// </summary>
public record UserBillsResponse
{
    public Guid UserExternalUserId { get; init; } 
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
    public List<UserBillSummaryDto> Bills { get; init; } = new();
    public UserBillsStatisticsDto Statistics { get; init; } = new();
}

/// <summary>
/// Summary of a bill for user bills response
/// </summary>
public record UserBillSummaryDto
{
    public Guid BillId { get; init; }
    public string BillNumber { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string ReferenceId { get; init; } = string.Empty;
    public string BillType { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public bool IsPaid { get; init; }
    public bool IsPartiallyPaid { get; init; }
    public bool IsOverdue { get; init; }
    public bool IsCancelled { get; init; }
    public decimal TotalAmountRials { get; init; }
    public decimal PaidAmountRials { get; init; }
    public decimal RemainingAmountRials { get; init; }
    public DateTime IssueDate { get; init; }
    public DateTime? DueDate { get; init; }
    public DateTime? FullyPaidDate { get; init; }
    public decimal PaymentCompletionPercentage { get; init; }
    public int DaysUntilDue { get; init; }
    public int DaysOverdue { get; init; }
    public int ItemsCount { get; init; }
    public int PaymentsCount { get; init; }
}

/// <summary>
/// Statistics for user bills
/// </summary>
public record UserBillsStatisticsDto
{
    public int TotalBills { get; init; }
    public int PaidBills { get; init; }
    public int UnpaidBills { get; init; }
    public int PartiallyPaidBills { get; init; }
    public int OverdueBills { get; init; }
    public int CancelledBills { get; init; }
    public decimal TotalAmountRials { get; init; }
    public decimal PaidAmountRials { get; init; }
    public decimal RemainingAmountRials { get; init; }
    public decimal OverdueAmountRials { get; init; }
}