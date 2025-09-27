using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Queries.Wallets;

/// <summary>
/// Query to get wallet transaction history
/// </summary>
public record GetWalletTransactionsQuery : IRequest<ApplicationResult<WalletTransactionsResponse>>
{
    /// <summary>
    /// User's national number
    /// </summary>
    public Guid ExternalUserId { get; init; }

    /// <summary>
    /// Filter by transaction type (optional)
    /// </summary>
    public string? TransactionType { get; init; }

    /// <summary>
    /// Filter by date range - start date (optional)
    /// </summary>
    public DateTime? FromDate { get; init; }

    /// <summary>
    /// Filter by date range - end date (optional)
    /// </summary>
    public DateTime? ToDate { get; init; }

    /// <summary>
    /// Filter by reference ID (optional)
    /// </summary>
    public string? ReferenceId { get; init; }

    /// <summary>
    /// Filter by external reference (optional)
    /// </summary>
    public string? ExternalReference { get; init; }

    /// <summary>
    /// Page number for pagination (default: 1)
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Page size for pagination (default: 20, max: 100)
    /// </summary>
    public int PageSize { get; init; } = 20;

    /// <summary>
    /// Sort by field (CreatedAt, Amount, TransactionType)
    /// </summary>
    public string SortBy { get; init; } = "CreatedAt";

    /// <summary>
    /// Sort direction (asc, desc)
    /// </summary>
    public string SortDirection { get; init; } = "desc";
}

/// <summary>
/// Response for GetWalletTransactionsQuery
/// </summary>
public record WalletTransactionsResponse
{
    public Guid WalletId { get; init; }
    public Guid UserExternalUserId { get; init; }
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
    public List<WalletTransactionDetailDto> Transactions { get; init; } = new();
    public WalletTransactionStatisticsDto Statistics { get; init; } = new();
}

/// <summary>
/// Detailed wallet transaction information
/// </summary>
public record WalletTransactionDetailDto
{
    public Guid TransactionId { get; init; }
    public string TransactionType { get; init; } = string.Empty;
    public decimal AmountRials { get; init; }
    public decimal BalanceBeforeRials { get; init; }
    public decimal PreviousBalanceRials { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public string ReferenceId { get; init; } = string.Empty;
    public string? ExternalReference { get; init; }
    public string? Description { get; init; }
    public Dictionary<string, string>? Metadata { get; init; }
}

/// <summary>
/// Wallet transaction statistics
/// </summary>
public record WalletTransactionStatisticsDto
{
    public int TotalTransactions { get; init; }
    public int DepositTransactions { get; init; }
    public int WithdrawalTransactions { get; init; }
    public int TransferInTransactions { get; init; }
    public int TransferOutTransactions { get; init; }
    public int PaymentTransactions { get; init; }
    public int RefundTransactions { get; init; }
    public int AdjustmentTransactions { get; init; }
    public decimal TotalDepositRials { get; init; }
    public decimal TotalWithdrawalRials { get; init; }
    public decimal TotalTransferInRials { get; init; }
    public decimal TotalTransferOutRials { get; init; }
    public decimal TotalPaymentRials { get; init; }
    public decimal TotalRefundRials { get; init; }
    public decimal TotalAdjustmentRials { get; init; }
}
