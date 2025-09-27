using FluentValidation;
using MediatR;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Finance.Application.Commands.Wallets;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Finance.Domain.Services;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Application.Features.Wallets.Commands.ChargeWallet;

/// <summary>
/// Handler for ChargeWalletCommand - Charges a wallet with a specific amount
/// </summary>
public class ChargeWalletCommandHandler : IRequestHandler<ChargeWalletCommand, ApplicationResult<ChargeWalletResponse>>
{
    private readonly IWalletRepository _walletRepository;
    private readonly IValidator<ChargeWalletCommand> _validator;
    private readonly IFinanceUnitOfWork _unitOfWork;
    private readonly WalletDomainService _walletDomainService;

    public ChargeWalletCommandHandler(
        IWalletRepository walletRepository,
        IValidator<ChargeWalletCommand> validator,
        IFinanceUnitOfWork unitOfWork,
        WalletDomainService walletDomainService)
    {
        _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _walletDomainService = walletDomainService ?? throw new ArgumentNullException(nameof(walletDomainService));
    }

    public async Task<ApplicationResult<ChargeWalletResponse>> Handle(
        ChargeWalletCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate the command
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return ApplicationResult<ChargeWalletResponse>.Failure(
                    validationResult.Errors.Select(e => e.ErrorMessage).ToList(),
                    "Validation failed"
                    );
            }

            // Get wallet by national number with refreshed balance
            var wallet = await _walletRepository.GetByExternalUserIdWithRefreshedBalanceAsync(
                request.ExternalUserId, cancellationToken);
            
            if (wallet == null)
            {
                return ApplicationResult<ChargeWalletResponse>.Failure(
                    "Wallet not found for this user");
            }

            // Check if wallet is active
            if (wallet.Status != WalletStatus.Active)
            {
                return ApplicationResult<ChargeWalletResponse>.Failure(
                    $"Cannot charge wallet. Current status: {wallet.Status}");
            }

            // Validate transaction limits
            var amount = Money.FromRials(request.AmountRials);
            var limitValidation = _walletDomainService.ValidateTransactionLimits(
                wallet, amount, WalletTransactionType.Deposit);

            if (!limitValidation.IsValid)
            {
                return ApplicationResult<ChargeWalletResponse>.Failure(
                    [limitValidation.ErrorMessage??""], 
                    "Transaction limit validation failed"
                    );
            }

            // Get previous balance from wallet
            var previousBalance = wallet.Balance;

            // Charge the wallet
            var transaction = wallet.Deposit(
                amount,
                request.ReferenceId,
                request.Description,
                request.ExternalReference,
                request.Metadata ?? new Dictionary<string, string>());

            // Update wallet in repository
            await _walletRepository.UpdateAsync(wallet, cancellationToken:cancellationToken);

            // Commit changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Get new balance from wallet (already updated by Deposit method)
            var newBalance = wallet.Balance;

             // Return response
             var response = new ChargeWalletResponse
             {
                 WalletId = wallet.Id,
                 TransactionId = transaction.Id,
                 UserExternalUserId = wallet.ExternalUserId,
                 AmountRials = amount.AmountRials,
                 PreviousBalanceRials = previousBalance.AmountRials,
                 NewBalanceRials = newBalance.AmountRials,
                 TransactionType = transaction.TransactionType.ToString(),
                 Status = "Completed", // Wallet transactions are always completed when created
                 TransactionDate = transaction.CreatedAt,
                 ReferenceId = transaction.ReferenceId ?? string.Empty,
                 ExternalReference = transaction.ExternalReference
             };

            return ApplicationResult<ChargeWalletResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return ApplicationResult<ChargeWalletResponse>.Failure(
                ex,
                "Failed to charge wallet"
                );
        }
    }
}
