using Microsoft.Extensions.Logging;
using Nezam.Refahi.Finance.Contracts.IntegrationEvents;
using Nezam.Refahi.Finance.Domain.Repositories;
using MassTransit;
using Nezam.Refahi.Finance.Application.Services;

namespace Nezam.Refahi.Finance.Application.Consumers;

/// <summary>
/// Sets WalletDeposit status to Pending when orchestrator signals bill creation.
/// </summary>
public class WalletDepositPendingConsumer : IConsumer<PendWalletDepositIntegrationEvent>
{
    private readonly IWalletDepositRepository _walletDepositRepository;
    private readonly IFinanceUnitOfWork _unitOfWork;
    private readonly ILogger<WalletDepositPendingConsumer> _logger;

    public WalletDepositPendingConsumer(
        IWalletDepositRepository walletDepositRepository,
        IFinanceUnitOfWork unitOfWork,
        ILogger<WalletDepositPendingConsumer> logger)
    {
        _walletDepositRepository = walletDepositRepository ?? throw new ArgumentNullException(nameof(walletDepositRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<PendWalletDepositIntegrationEvent> context)
    {
        var notification = context.Message;
        var cancellationToken = context.CancellationToken;

        _logger.LogInformation("Wallet deposit ready: TrackingCode {TrackingCode}, Bill {BillNumber}", notification.TrackingCode, notification.BillNumber);

        var deposit = await _walletDepositRepository.GetByTrackingCodeAsync(notification.TrackingCode, cancellationToken);
        if (deposit == null)
        {
            _logger.LogWarning("Wallet deposit not found for TrackingCode {TrackingCode}", notification.TrackingCode);
            return;
        }

        await _unitOfWork.BeginAsync(cancellationToken);
        try
        {
            deposit.MarkPending();
            await _walletDepositRepository.UpdateAsync(deposit, cancellationToken: cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Failed to mark wallet deposit {DepositId} as Pending", deposit.Id);
        }
    }
}


