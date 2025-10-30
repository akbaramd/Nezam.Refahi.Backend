using Microsoft.Extensions.Logging;
using Nezam.Refahi.Contracts.Finance.v1.Messages;
using MassTransit;
using MediatR;
using Nezam.Refahi.Finance.Application.Commands.Wallets;

namespace Nezam.Refahi.Finance.Application.Consumers;

/// <summary>
/// Sets WalletDeposit status to Pending when orchestrator signals bill creation.
/// </summary>
public class MarkWalletDepositPendingMessageConsumer : IConsumer<MarkWalletDepositPendingCommandMessage>
{
    private readonly IMediator _mediator;
    private readonly ILogger<MarkWalletDepositPendingMessageConsumer> _logger;

    public MarkWalletDepositPendingMessageConsumer(
        IMediator mediator,
        ILogger<MarkWalletDepositPendingMessageConsumer> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<MarkWalletDepositPendingCommandMessage> context)
    {
        var notification = context.Message;
        var cancellationToken = context.CancellationToken;

        _logger.LogInformation("Wallet deposit ready to mark pending: DepositId {DepositId}, Bill {BillNumber}", notification.WalletDepositId, notification.BillNumber);

        var cmd = new MarkWalletDepositPendingCommand { WalletDepositId = notification.WalletDepositId };
        await _mediator.Send(cmd, cancellationToken);
    }
}


