using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Finance.Application.Commands.Wallets;
using Nezam.Refahi.Finance.Contracts.IntegrationEvents;
using Nezam.Refahi.Finance.Domain.Repositories;
using MassTransit;

namespace Nezam.Refahi.Finance.Application.Consumers;

/// <summary>
/// Handles wallet charging when the orchestrator publishes WalletDepositCompletedIntegrationEvent.
/// </summary>
public class WalletDepositCompletedConsumer : IConsumer<WalletDepositCompletedIntegrationEvent>
{
    private readonly IMediator _mediator;
    private readonly IWalletDepositRepository _walletDepositRepository;
    private readonly ILogger<WalletDepositCompletedConsumer> _logger;

    public WalletDepositCompletedConsumer(
        IMediator mediator,
        IWalletDepositRepository walletDepositRepository,
        ILogger<WalletDepositCompletedConsumer> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _walletDepositRepository = walletDepositRepository ?? throw new ArgumentNullException(nameof(walletDepositRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<WalletDepositCompletedIntegrationEvent> context)
    {
        var notification = context.Message;
        var cancellationToken = context.CancellationToken;

        _logger.LogInformation("Handling WalletDepositCompletedIntegrationEvent for TrackingCode {TrackingCode}", notification.TrackingCode);

        var deposit = await _walletDepositRepository.GetByTrackingCodeAsync(notification.TrackingCode, cancellationToken);
        if (deposit == null)
        {
            _logger.LogWarning("Deposit not found for TrackingCode {TrackingCode}", notification.TrackingCode);
            return;
        }

        // Complete deposit if still pending
        if (deposit.Status == Domain.Enums.WalletDepositStatus.Pending)
        {
            deposit.Complete();
            await _walletDepositRepository.UpdateAsync(deposit, cancellationToken: cancellationToken);
        }

        // Charge wallet via command
        var chargeCommand = new ChargeWalletCommand
        {
            ExternalUserId = notification.ExternalUserId,
            AmountRials = deposit.Amount.AmountRials,
            ReferenceId = deposit.Id.ToString(),
            ExternalReference = notification.PaymentId.ToString(),
            Description = $"واریز کیف پول - صورتحساب {notification.BillNumber}",
            Metadata = new Dictionary<string, string>
            {
                ["DepositId"] = deposit.Id.ToString(),
                ["TrackingCode"] = notification.TrackingCode,
                ["BillId"] = notification.BillId.ToString(),
                ["BillNumber"] = notification.BillNumber,
                ["PaidAt"] = notification.PaidAt.ToString("O")
            }
        };

        await _mediator.Send(chargeCommand, cancellationToken);
    }
}


