using FluentValidation;
using MediatR;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Finance.Contracts.Queries.Wallets;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Finance.Domain.Services;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Features.Wallets.Queries.GetWalletBalance;

/// <summary>
/// Handler for GetWalletBalanceQuery - Retrieves wallet balance information
/// </summary>
public class GetWalletBalanceQueryHandler : IRequestHandler<GetWalletBalanceQuery, ApplicationResult<WalletBalanceResponse>>
{
    private readonly IWalletRepository _walletRepository;
    private readonly IWalletTransactionRepository _walletTransactionRepository;
    private readonly IValidator<GetWalletBalanceQuery> _validator;
    private readonly WalletDomainService _walletDomainService;
    private readonly IFinanceUnitOfWork _unitOfWork;

    public GetWalletBalanceQueryHandler(
        IWalletRepository walletRepository,
        IWalletTransactionRepository walletTransactionRepository,
        IValidator<GetWalletBalanceQuery> validator,
        WalletDomainService walletDomainService,
        IFinanceUnitOfWork unitOfWork)
    {
        _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        _walletTransactionRepository = walletTransactionRepository ?? throw new ArgumentNullException(nameof(walletTransactionRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _walletDomainService = walletDomainService ?? throw new ArgumentNullException(nameof(walletDomainService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ApplicationResult<WalletBalanceResponse>> Handle(
        GetWalletBalanceQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate the query
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return ApplicationResult<WalletBalanceResponse>.Failure(
                    validationResult.Errors.Select(e => e.ErrorMessage).ToList(),
                    "Validation failed"
                    );
            }

            // Get wallet by national number with refreshed balance
            var wallet = await _walletRepository.GetByExternalUserIdWithRefreshedBalanceAsync(
                request.ExternalUserId, cancellationToken);
            
            // If wallet doesn't exist, create a new blank wallet for the user
            if (wallet == null)
            {
                wallet = new Wallet(
                    externalUserId: request.ExternalUserId,
                    walletName: $"کیف پول {request.ExternalUserId}",
                    description: "کیف پول پیش‌فرض کاربر",
                    metadata: new Dictionary<string, string>
                    {
                        { "created_by", "system" },
                        { "created_reason", "auto_created_on_balance_check" }
                    });

                // Save the new wallet
                await _walletRepository.AddAsync(wallet, cancellationToken: cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            // Build response
            var response = new WalletBalanceResponse
            {
                WalletId = wallet.Id,
                UserExternalUserId = wallet.ExternalUserId,
                UserFullName = wallet.WalletName ?? string.Empty,
                CurrentBalanceRials = wallet.Balance.AmountRials,
                Status = wallet.Status.ToString(),
                CreatedAt = wallet.CreatedAt,
                LastTransactionAt = wallet.LastTransactionAt
            };

            // Include transaction history if requested
            if (request.IncludeTransactionHistory)
            {
                var recentTransactions = await _walletTransactionRepository.GetByWalletIdAsync(
                    wallet.Id, 1, request.TransactionHistoryCount, cancellationToken);

                response = response with
                {
                    RecentTransactions = recentTransactions.Select(t => new WalletTransactionSummaryDto
                    {
                        TransactionId = t.Id,
                        TransactionType = t.TransactionType.ToString(),
                        AmountRials = t.Amount.AmountRials,
                        PreviousBalanceRials = t.PreviousBalance.AmountRials,
                        Status = "Completed", // Wallet transactions are always completed when created
                        CreatedAt = t.CreatedAt,
                        ReferenceId = t.ReferenceId ?? string.Empty,
                        ExternalReference = t.ExternalReference,
                        Description = t.Description
                    }).ToList()
                };

                // Set last transaction if available
                if (recentTransactions.Any())
                {
                    var lastTransaction = recentTransactions.First();
                    response = response with
                    {
                        LastTransaction = new WalletTransactionSummaryDto
                        {
                            TransactionId = lastTransaction.Id,
                            TransactionType = lastTransaction.TransactionType.ToString(),
                            AmountRials = lastTransaction.Amount.AmountRials,
                            PreviousBalanceRials = lastTransaction.PreviousBalance.AmountRials,
                            Status = "Completed", // Wallet transactions are always completed when created
                            CreatedAt = lastTransaction.CreatedAt,
                            ReferenceId = lastTransaction.ReferenceId ?? string.Empty,
                            ExternalReference = lastTransaction.ExternalReference,
                            Description = lastTransaction.Description
                        }
                    };
                }
            }

            // Include balance analysis if requested
            if (request.IncludeBalanceAnalysis)
            {
                var analysis = _walletDomainService.AnalyzeBalanceHistory(
                    wallet, 
                    DateTime.UtcNow.AddDays(-request.AnalysisDays));

                response = response with
                {
                    BalanceAnalysis = new WalletBalanceAnalysisDto
                    {
                        StartingBalanceRials = analysis.StartingBalance.AmountRials,
                        EndingBalanceRials = analysis.EndingBalance.AmountRials,
                        TotalInflowRials = analysis.TotalInflow.AmountRials,
                        TotalOutflowRials = analysis.TotalOutflow.AmountRials,
                        TotalTransactions = analysis.TransactionCount,
                        TrendPoints = analysis.TrendPoints.Select(tp => new BalanceTrendPointDto
                        {
                            Date = tp.Date,
                            BalanceRials = tp.Balance.AmountRials,
                            DailyChangeRials = tp.DailyChange.AmountRials
                        }).ToList()
                    }
                };
            }

            return ApplicationResult<WalletBalanceResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return ApplicationResult<WalletBalanceResponse>.Failure(
                ex,
                "خطا در دریافت موجودی کیف پول"
                );
        }
    }
}
