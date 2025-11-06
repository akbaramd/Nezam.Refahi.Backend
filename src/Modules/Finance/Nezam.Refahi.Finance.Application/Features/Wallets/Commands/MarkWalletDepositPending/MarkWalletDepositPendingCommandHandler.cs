using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Finance.Application.Commands.Wallets;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;
using MassTransit;
using Nezam.Refahi.Contracts.Finance.v1.Messages;
using Nezam.Refahi.Finance.Contracts.IntegrationEvents;

namespace Nezam.Refahi.Finance.Application.Features.Wallets.Commands.MarkWalletDepositPending;

public class MarkWalletDepositAwaitingPaymentCommandHandler : IRequestHandler<MarkWalletDepositAwaitingPaymentCommand, ApplicationResult<Unit>>
{
    private readonly IWalletDepositRepository _walletDepositRepository;
    private readonly IFinanceUnitOfWork _unitOfWork;
    private readonly ILogger<MarkWalletDepositAwaitingPaymentCommandHandler> _logger;
    private readonly IBus _publishEndpoint;

    public MarkWalletDepositAwaitingPaymentCommandHandler(
        IWalletDepositRepository walletDepositRepository,
        IFinanceUnitOfWork unitOfWork,
        IBus publishEndpoint,
        ILogger<MarkWalletDepositAwaitingPaymentCommandHandler> logger)
    {
        _walletDepositRepository = walletDepositRepository ?? throw new ArgumentNullException(nameof(walletDepositRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult<Unit>> Handle(MarkWalletDepositAwaitingPaymentCommand request, CancellationToken cancellationToken)
    {
        var deposit = await _walletDepositRepository.GetByIdAsync(request.WalletDepositId, cancellationToken);
        if (deposit == null)
        {
            _logger.LogWarning("Wallet deposit not found for Id {DepositId}", request.WalletDepositId);
            return ApplicationResult<Unit>.Failure("Wallet deposit not found");
        }

        await _unitOfWork.BeginAsync(cancellationToken);
        try
        {
            deposit.MarkAwaitingPayment();
            await _walletDepositRepository.UpdateAsync(deposit, cancellationToken: cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return ApplicationResult<Unit>.Success(Unit.Value);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Failed to mark wallet deposit {DepositId} as AwaitingPayment", deposit.Id);
            // Publish failure integration event
            var failedEvent = new WalletDepositPendingFailedEventMessage()
            {
                WalletDepositId = deposit.Id,
                TrackingCode = deposit.TrackingCode,
                FailureReason = ex.Message,
                Metadata = new Dictionary<string, string>
                {
                    ["DepositId"] = deposit.Id.ToString()
                }
            };
            await _publishEndpoint.Publish(failedEvent, cancellationToken);

            return ApplicationResult<Unit>.Failure(ex, "Failed to mark deposit awaiting payment");
        }
    }
}


