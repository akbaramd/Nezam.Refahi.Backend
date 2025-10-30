using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Finance.Application.Commands.Wallets;
using Nezam.Refahi.Contracts.Finance.v1.Messages;
using MassTransit;

namespace Nezam.Refahi.Finance.Application.Consumers;

/// <summary>
/// Handles rollback when wallet deposit fails at any stage.
/// Thin consumer: forwards to HandleFailedWalletDepositCommand.
/// </summary>
public class FailWalletDepositMessageConsumer : IConsumer<FailWalletDepositCommandMessage>
{
    private readonly IMediator _mediator;
    private readonly ILogger<FailWalletDepositMessageConsumer> _logger;

    public FailWalletDepositMessageConsumer(IMediator mediator, ILogger<FailWalletDepositMessageConsumer> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<FailWalletDepositCommandMessage> context)
    {
        var msg = context.Message;
        var cancellationToken = context.CancellationToken;

        _logger.LogWarning("Handling FailWalletDepositCommandMessage for DepositId {DepositId} Stage {Stage}", msg.WalletDepositId, msg.FailureStage);

        var cmd = new HandleFailedWalletDepositCommand
        {
            WalletDepositId = msg.WalletDepositId,
            FailureStage = msg.FailureStage,
            FailureReason = msg.FailureReason,
            ErrorCode = msg.ErrorCode,
            BillId = msg.BillId,
            BillNumber = msg.BillNumber,
            PaymentId = msg.PaymentId
        };

        await _mediator.Send(cmd, cancellationToken);
    }
}


