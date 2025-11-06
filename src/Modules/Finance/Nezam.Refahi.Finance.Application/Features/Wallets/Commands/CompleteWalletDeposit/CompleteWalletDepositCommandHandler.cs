using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Finance.Application.Commands.Wallets;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;
using MassTransit;
using Nezam.Refahi.Contracts.Finance.v1.Messages;
using Nezam.Refahi.Finance.Contracts.IntegrationEvents;

namespace Nezam.Refahi.Finance.Application.Features.Wallets.Commands.CompleteWalletDeposit;

public class CompleteWalletDepositCommandHandler : IRequestHandler<CompleteWalletDepositCommand, ApplicationResult<Unit>>
{
    private readonly IWalletDepositRepository _walletDepositRepository;
    private readonly IFinanceUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly ILogger<CompleteWalletDepositCommandHandler> _logger;
    private readonly IBus _publishEndpoint;

    public CompleteWalletDepositCommandHandler(
        IWalletDepositRepository walletDepositRepository,
        IFinanceUnitOfWork unitOfWork,
        IMediator mediator,
        IBus publishEndpoint,
        ILogger<CompleteWalletDepositCommandHandler> logger)
    {
        _walletDepositRepository = walletDepositRepository ?? throw new ArgumentNullException(nameof(walletDepositRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult<Unit>> Handle(CompleteWalletDepositCommand request, CancellationToken cancellationToken)
    {
        var deposit = await _walletDepositRepository.GetByIdAsync(request.WalletDepositId, cancellationToken);
        if (deposit == null)
        {
            _logger.LogWarning("Wallet deposit not found for Id {DepositId}", request.WalletDepositId);
            return ApplicationResult<Unit>.Failure("Wallet deposit not found");
        }

        // Mark completed if still pending
        if (deposit.Status == WalletDepositStatus.AwaitingPayment)
        {
            await _unitOfWork.BeginAsync(cancellationToken);
            try
            {
                deposit.Complete();
                await _walletDepositRepository.UpdateAsync(deposit, cancellationToken: cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Failed to mark wallet deposit {DepositId} as Completed", deposit.Id);
                // Publish failure integration event
                var failedEvent = new WalletDepositCompletionFailedMessage()
                {
                    WalletDepositId = deposit.Id,
                    TrackingCode = deposit.TrackingCode,
                    BillId = request.BillId,
                    BillNumber = request.BillNumber,
                    PaymentId = request.PaymentId,
                    FailureReason = ex.Message,
                    Metadata = new Dictionary<string, string>
                    {
                        ["DepositId"] = deposit.Id.ToString()
                    }
                };
                await _publishEndpoint.Publish(failedEvent, cancellationToken);
                return ApplicationResult<Unit>.Failure(ex, "Failed to mark deposit completed");
            }
        }

        // Charge wallet
        var chargeCommand = new ChargeWalletCommand
        {
            ExternalUserId = request.ExternalUserId,
            AmountRials = deposit.Amount.AmountRials,
            ReferenceId = deposit.Id.ToString(),
            ExternalReference = request.PaymentId.ToString(),
            Description = $"واریز کیف پول - صورتحساب {request.BillNumber}",
            Metadata = new Dictionary<string, string>
            {
                ["DepositId"] = deposit.Id.ToString(),
                ["TrackingCode"] = request.TrackingCode,
                ["BillId"] = request.BillId.ToString(),
                ["BillNumber"] = request.BillNumber,
                ["PaidAt"] = request.PaidAt.ToString("O")
            }
        };

        var result = await _mediator.Send(chargeCommand, cancellationToken);
        if (!result.IsSuccess)
        {
            return ApplicationResult<Unit>.Failure(result.Errors, result.Message ?? "Wallet charge failed");
        }

        // Publish completion event for orchestrator saga finalization
        var completedEvent = new WalletDepositCompletedEventMessage
        {
            WalletDepositId = deposit.Id,
            TrackingCode = request.TrackingCode,
            ExternalUserId = request.ExternalUserId,
            UserFullName = deposit.Metadata.TryGetValue("UserFullName", out var name) ? name : string.Empty,
            AmountRials = deposit.Amount.AmountRials,
            Currency = deposit.Amount.Currency,
            BillId = request.BillId,
            BillNumber = request.BillNumber,
            PaymentId = request.PaymentId,
            CompletedAt = request.PaidAt,
            Metadata = new Dictionary<string, string>
            {
                ["DepositId"] = deposit.Id.ToString(),
                ["TrackingCode"] = request.TrackingCode,
                ["BillId"] = request.BillId.ToString(),
                ["BillNumber"] = request.BillNumber
            }
        };
        await _publishEndpoint.Publish(completedEvent, cancellationToken);

        return ApplicationResult<Unit>.Success(Unit.Value);
    }
}


