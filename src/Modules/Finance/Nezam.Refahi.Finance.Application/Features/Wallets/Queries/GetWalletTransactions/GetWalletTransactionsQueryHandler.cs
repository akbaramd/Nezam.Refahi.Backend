using FluentValidation;
using MediatR;
using Nezam.Refahi.Finance.Contracts.Queries.Wallets;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Features.Wallets.Queries.GetWalletTransactions;

/// <summary>
/// Handler for GetWalletTransactionsQuery - Retrieves wallet transaction history
/// </summary>
public class GetWalletTransactionsQueryHandler : IRequestHandler<GetWalletTransactionsQuery, ApplicationResult<WalletTransactionsResponse>>
{
    private readonly IWalletRepository _walletRepository;
    private readonly IWalletTransactionRepository _walletTransactionRepository;
    private readonly IValidator<GetWalletTransactionsQuery> _validator;

    public GetWalletTransactionsQueryHandler(
        IWalletRepository walletRepository,
        IWalletTransactionRepository walletTransactionRepository,
        IValidator<GetWalletTransactionsQuery> validator)
    {
        _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        _walletTransactionRepository = walletTransactionRepository ?? throw new ArgumentNullException(nameof(walletTransactionRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<ApplicationResult<WalletTransactionsResponse>> Handle(
        GetWalletTransactionsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate the query
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return ApplicationResult<WalletTransactionsResponse>.Failure(
                    validationResult.Errors.Select(e => e.ErrorMessage).ToList(),
                    "Validation failed"
                    );
            }

            // Get wallet by national number
            var wallet = await _walletRepository.GetByExternalUserIdAsync(
                request.ExternalUserId, cancellationToken);
            
            if (wallet == null)
            {
                return ApplicationResult<WalletTransactionsResponse>.Failure(
                    "Wallet not found for this user");
            }

            // Parse transaction type filter
            WalletTransactionType? transactionTypeFilter = null;
            if (!string.IsNullOrEmpty(request.TransactionType) && 
                Enum.TryParse<WalletTransactionType>(request.TransactionType, out var parsedType))
            {
                transactionTypeFilter = parsedType;
            }

            // Get transactions based on filters
            IEnumerable<Domain.Entities.WalletTransaction> transactions;

            if (request.FromDate.HasValue && request.ToDate.HasValue)
            {
                transactions = await _walletTransactionRepository.GetByWalletIdAndDateRangeAsync(
                    wallet.Id, request.FromDate.Value, request.ToDate.Value, cancellationToken);
            }
            else if (transactionTypeFilter.HasValue)
            {
                transactions = await _walletTransactionRepository.GetByWalletIdAndTypeAsync(
                    wallet.Id, transactionTypeFilter.Value, cancellationToken);
            }
            else
            {
                // Get paginated transactions
                var pageNumber = Math.Max(1, request.PageNumber);
                var pageSize = Math.Min(100, Math.Max(1, request.PageSize));
                
                transactions = await _walletTransactionRepository.GetByWalletIdAsync(
                    wallet.Id, pageNumber, pageSize, cancellationToken);
            }

            // Apply additional filters
            if (!string.IsNullOrEmpty(request.ReferenceId))
            {
                transactions = transactions.Where(t => t.ReferenceId?.Contains(request.ReferenceId) ?? false);
            }

            if (!string.IsNullOrEmpty(request.ExternalReference))
            {
                transactions = transactions.Where(t => 
                    !string.IsNullOrEmpty(t.ExternalReference) && 
                    t.ExternalReference.Contains(request.ExternalReference));
            }

            // Sort transactions
            var sortedTransactions = request.SortBy.ToLower() switch
            {
                "amount" => request.SortDirection.ToLower() == "asc" 
                    ? transactions.OrderBy(t => t.Amount.AmountRials)
                    : transactions.OrderByDescending(t => t.Amount.AmountRials),
                "transactiontype" => request.SortDirection.ToLower() == "asc"
                    ? transactions.OrderBy(t => t.TransactionType)
                    : transactions.OrderByDescending(t => t.TransactionType),
                _ => request.SortDirection.ToLower() == "asc"
                    ? transactions.OrderBy(t => t.CreatedAt)
                    : transactions.OrderByDescending(t => t.CreatedAt)
            };

            var transactionList = sortedTransactions.ToList();

            // Calculate statistics
            var statistics = await _walletTransactionRepository.GetTransactionTypeStatisticsAsync(
                wallet.Id, request.FromDate, request.ToDate, cancellationToken);

            // Build response
            var response = new WalletTransactionsResponse
            {
                WalletId = wallet.Id,
                UserExternalUserId = wallet.ExternalUserId,
                TotalCount = transactionList.Count,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)transactionList.Count / request.PageSize),
                Transactions = transactionList.Select(t => new WalletTransactionDetailDto
                {
                    TransactionId = t.Id,
                    TransactionType = t.TransactionType.ToString(),
                    AmountRials = t.Amount.AmountRials,
                    BalanceBeforeRials = t.PreviousBalance.AmountRials, // Previous balance before this transaction
                    PreviousBalanceRials = t.PreviousBalance.AmountRials,
                    Status = "Completed", // Wallet transactions are always completed when created
                    CreatedAt = t.CreatedAt,
                    ReferenceId = t.ReferenceId ?? string.Empty,
                    ExternalReference = t.ExternalReference,
                    Description = t.Description,
                    Metadata = t.Metadata
                }).ToList(),
                Statistics = new WalletTransactionStatisticsDto
                {
                    TotalTransactions = statistics.Values.Sum(),
                    DepositTransactions = statistics.GetValueOrDefault(WalletTransactionType.Deposit, 0),
                    WithdrawalTransactions = statistics.GetValueOrDefault(WalletTransactionType.Withdrawal, 0),
                    TransferInTransactions = statistics.GetValueOrDefault(WalletTransactionType.TransferIn, 0),
                    TransferOutTransactions = statistics.GetValueOrDefault(WalletTransactionType.TransferOut, 0),
                    PaymentTransactions = statistics.GetValueOrDefault(WalletTransactionType.Payment, 0),
                    RefundTransactions = statistics.GetValueOrDefault(WalletTransactionType.Refund, 0),
                    AdjustmentTransactions = statistics.GetValueOrDefault(WalletTransactionType.Adjustment, 0),
                    TotalDepositRials = transactionList.Where(t => t.TransactionType == WalletTransactionType.Deposit).Sum(t => t.Amount.AmountRials),
                    TotalWithdrawalRials = transactionList.Where(t => t.TransactionType == WalletTransactionType.Withdrawal).Sum(t => t.Amount.AmountRials),
                    TotalTransferInRials = transactionList.Where(t => t.TransactionType == WalletTransactionType.TransferIn).Sum(t => t.Amount.AmountRials),
                    TotalTransferOutRials = transactionList.Where(t => t.TransactionType == WalletTransactionType.TransferOut).Sum(t => t.Amount.AmountRials),
                    TotalPaymentRials = transactionList.Where(t => t.TransactionType == WalletTransactionType.Payment).Sum(t => t.Amount.AmountRials),
                    TotalRefundRials = transactionList.Where(t => t.TransactionType == WalletTransactionType.Refund).Sum(t => t.Amount.AmountRials),
                    TotalAdjustmentRials = transactionList.Where(t => t.TransactionType == WalletTransactionType.Adjustment).Sum(t => t.Amount.AmountRials)
                }
            };

            return ApplicationResult<WalletTransactionsResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return ApplicationResult<WalletTransactionsResponse>.Failure(
                ex,
                "Failed to retrieve wallet transactions"
                );
        }
    }
}
