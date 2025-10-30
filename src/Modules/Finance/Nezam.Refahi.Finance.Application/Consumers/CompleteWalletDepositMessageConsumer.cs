using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Finance.Application.Commands.Wallets;
using Nezam.Refahi.Contracts.Finance.v1.Messages;
using MassTransit;

namespace Nezam.Refahi.Finance.Application.Consumers;

/// <summary>
/// Handles wallet charging when the orchestrator publishes WalletDepositCompletedIntegrationEvent.
/// </summary>
public class CompleteWalletDepositMessageConsumer : IConsumer<CompleteWalletDepositCommandMessage>
{
    private readonly IMediator _mediator;
    private readonly ILogger<CompleteWalletDepositMessageConsumer> _logger;

    public CompleteWalletDepositMessageConsumer(
        IMediator mediator,
        ILogger<CompleteWalletDepositMessageConsumer> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<CompleteWalletDepositCommandMessage> context)
    {
        var notification = context.Message;
        var cancellationToken = context.CancellationToken;

        _logger.LogInformation("Handling WalletDepositCompletedIntegrationEvent for TrackingCode {TrackingCode}", notification.TrackingCode);

        var cmd = new CompleteWalletDepositCommand
        {
            WalletDepositId = notification.WalletDepositId,
            ExternalUserId = notification.ExternalUserId,
            BillId = notification.BillId,
            BillNumber = notification.BillNumber,
            PaymentId = notification.PaymentId,
            PaidAt = notification.PaidAt,
            TrackingCode = notification.TrackingCode
        };

        await _mediator.Send(cmd, cancellationToken);
    }
}


