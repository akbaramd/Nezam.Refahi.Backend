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

namespace Nezam.Refahi.Finance.Application.Features.Wallets.Commands.CreateWallet;

/// <summary>
/// Handler for CreateWalletCommand - Creates a new wallet for a user
/// </summary>
public class CreateWalletCommandHandler : IRequestHandler<CreateWalletCommand, ApplicationResult<CreateWalletResponse>>
{
    private readonly IWalletRepository _walletRepository;
    private readonly IValidator<CreateWalletCommand> _validator;
    private readonly IFinanceUnitOfWork _unitOfWork;
    private readonly WalletDomainService _walletDomainService;

    public CreateWalletCommandHandler(
        IWalletRepository walletRepository,
        IValidator<CreateWalletCommand> validator,
        IFinanceUnitOfWork unitOfWork,
        WalletDomainService walletDomainService)
    {
        _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _walletDomainService = walletDomainService ?? throw new ArgumentNullException(nameof(walletDomainService));
    }

    public async Task<ApplicationResult<CreateWalletResponse>> Handle(
        CreateWalletCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate the command
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return ApplicationResult<CreateWalletResponse>.Failure(
                    validationResult.Errors.Select(e => e.ErrorMessage).ToList(),
                    "Validation failed"
                    );
            }

            // Check if wallet already exists for this user
            var existingWallet = await _walletRepository.GetByExternalUserIdAsync(
                request.ExternalUserId, cancellationToken);
            
            if (existingWallet != null)
            {
                return ApplicationResult<CreateWalletResponse>.Failure(
                    "Wallet already exists for this user");
            }

            // Create new wallet
            var wallet = new Wallet(
                request.ExternalUserId,
                request.UserFullName, // This will be used as walletName
                request.Description,
                request.Metadata);

            // Add wallet to repository
            await _walletRepository.AddAsync(wallet, cancellationToken:cancellationToken); 

            // Commit changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            // Return response
            var response = new CreateWalletResponse
            {
                WalletId = wallet.Id,
                UserExternalUserId = wallet.ExternalUserId,
                UserFullName = wallet.WalletName ?? request.UserFullName,
                InitialBalanceRials = wallet.Balance.AmountRials, // Initial balance is zero
                Status = wallet.Status.ToString(),
                CreatedAt = wallet.CreatedAt
            };

            return ApplicationResult<CreateWalletResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return ApplicationResult<CreateWalletResponse>.Failure(
                ex,
                "Failed to create wallet"
                );  
        }
    }
}
