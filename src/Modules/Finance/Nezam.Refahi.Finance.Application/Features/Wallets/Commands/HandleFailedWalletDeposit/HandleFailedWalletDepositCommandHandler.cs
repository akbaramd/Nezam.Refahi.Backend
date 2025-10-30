using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Finance.Application.Commands.Wallets;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Features.Wallets.Commands.HandleFailedWalletDeposit;

public class HandleFailedWalletDepositCommandHandler : IRequestHandler<HandleFailedWalletDepositCommand, ApplicationResult<Unit>>
{
    private readonly IWalletDepositRepository _walletDepositRepository;
    private readonly IFinanceUnitOfWork _unitOfWork;
    private readonly ILogger<HandleFailedWalletDepositCommandHandler> _logger;

    public HandleFailedWalletDepositCommandHandler(
        IWalletDepositRepository walletDepositRepository,
        IFinanceUnitOfWork unitOfWork,
        ILogger<HandleFailedWalletDepositCommandHandler> logger)
    {
        _walletDepositRepository = walletDepositRepository ?? throw new ArgumentNullException(nameof(walletDepositRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult<Unit>> Handle(HandleFailedWalletDepositCommand request, CancellationToken cancellationToken)
    {
        var deposit = await _walletDepositRepository.GetByIdAsync(request.WalletDepositId, cancellationToken);
        if (deposit == null)
        {
            _logger.LogWarning("Wallet deposit not found for Id {DepositId}", request.WalletDepositId);
            return ApplicationResult<Unit>.Failure("Wallet deposit not found");
        }

        // Record failure metadata always
        await _unitOfWork.BeginAsync(cancellationToken);
        try
        {
            deposit.AddMetadata("LastFailureStage", request.FailureStage);
            deposit.AddMetadata("LastFailureReason", request.FailureReason);
            if (!string.IsNullOrWhiteSpace(request.ErrorCode))
                deposit.AddMetadata("LastErrorCode", request.ErrorCode!);

            // Conservative rollback: if currently Pending, cancel; if Processing, just record failure
            if (deposit.Status == WalletDepositStatus.Pending)
            {
                deposit.Cancel(request.FailureReason);
            }

            await _walletDepositRepository.UpdateAsync(deposit, cancellationToken: cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return ApplicationResult<Unit>.Success(Unit.Value);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Failed to process failed wallet deposit {DepositId}", deposit.Id);
            return ApplicationResult<Unit>.Failure(ex, "Failed to process failed wallet deposit");
        }
    }
}


