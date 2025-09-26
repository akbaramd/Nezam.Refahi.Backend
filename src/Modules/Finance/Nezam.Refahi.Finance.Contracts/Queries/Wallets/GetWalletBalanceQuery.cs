using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Contracts.Queries.Wallets;

/// <summary>
/// Query to get wallet balance information
/// </summary>
public record GetWalletBalanceQuery : IRequest<ApplicationResult<WalletBalanceResponse>>
{
    /// <summary>
    /// User's national number
    /// </summary>
    public Guid ExternalUserId { get; init; }

    /// <summary>
    /// Include transaction history (optional)
    /// </summary>
    public bool IncludeTransactionHistory { get; init; } = false;

    /// <summary>
    /// Number of recent transactions to include (default: 10, max: 50)
    /// </summary>
    public int TransactionHistoryCount { get; init; } = 10;

    /// <summary>
    /// Include balance analysis (optional)
    /// </summary>
    public bool IncludeBalanceAnalysis { get; init; } = false;

    /// <summary>
    /// Number of days for balance analysis (default: 30)
    /// </summary>
    public int AnalysisDays { get; init; } = 30;
}

/// <summary>
/// Response for GetWalletBalanceQuery
/// </summary>
public record WalletBalanceResponse
{
    public Guid WalletId { get; init; }
    public Guid UserExternalUserId { get; init; }
    public string UserFullName { get; init; } = string.Empty;
    public decimal CurrentBalanceRials { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? LastTransactionAt { get; init; }
    public WalletTransactionSummaryDto? LastTransaction { get; init; }
    public List<WalletTransactionSummaryDto>? RecentTransactions { get; init; }
    public WalletBalanceAnalysisDto? BalanceAnalysis { get; init; }
}

/// <summary>
/// Summary of a wallet transaction
/// </summary>
public record WalletTransactionSummaryDto
{
    public Guid TransactionId { get; init; }
    public string TransactionType { get; init; } = string.Empty;
    public decimal AmountRials { get; init; }
    public decimal PreviousBalanceRials { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public string ReferenceId { get; init; } = string.Empty;
    public string? ExternalReference { get; init; }
    public string? Description { get; init; }
}

/// <summary>
/// Wallet balance analysis data
/// </summary>
public record WalletBalanceAnalysisDto
{
    public decimal StartingBalanceRials { get; init; }
    public decimal EndingBalanceRials { get; init; }
    public decimal TotalInflowRials { get; init; }
    public decimal TotalOutflowRials { get; init; }
    public int TotalTransactions { get; init; }
    public List<BalanceTrendPointDto> TrendPoints { get; init; } = new();
}

/// <summary>
/// Balance trend point for analysis
/// </summary>
public record BalanceTrendPointDto
{
    public DateTime Date { get; init; }
    public decimal BalanceRials { get; init; }
    public decimal DailyChangeRials { get; init; }
}
