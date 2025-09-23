using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Finance.Application.Features.Wallets.Commands.ChargeWallet;
using Nezam.Refahi.Finance.Contracts.Commands.Wallets;
using Nezam.Refahi.Finance.Domain.Events;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Infrastructure.Consumers;

/// <summary>
/// Event consumer that charges wallet when a wallet charge bill payment is completed
/// </summary>
public class WalletChargePaymentCompletedConsumer : INotificationHandler<BillFullyPaidEvent>
{
    private readonly IMediator _mediator;
    private readonly IBillRepository _billRepository;
    private readonly IWalletDepositRepository _walletDepositRepository;
    private readonly ILogger<WalletChargePaymentCompletedConsumer> _logger;

    public WalletChargePaymentCompletedConsumer(
        IMediator mediator,
        IBillRepository billRepository,
        IWalletDepositRepository walletDepositRepository,
        ILogger<WalletChargePaymentCompletedConsumer> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _walletDepositRepository = walletDepositRepository ?? throw new ArgumentNullException(nameof(walletDepositRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles BillFullyPaidEvent for wallet charge bills
    /// </summary>
    public async Task Handle(BillFullyPaidEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Processing bill fully paid event for BillId: {BillId}, ReferenceId: {ReferenceId}, ReferenceType: {ReferenceType}",
                notification.BillId, notification.ReferenceId, notification.ReferenceType);

            // Check if this is a wallet deposit bill by ReferenceType
            if (!IsWalletDepositBill(notification.ReferenceType))
            {
                _logger.LogDebug(
                    "Bill {BillId} is not a wallet deposit bill (ReferenceType: {ReferenceType}), skipping wallet charge",
                    notification.BillId, notification.ReferenceType);
                return;
            }

            // Find the deposit using the ReferenceId (which is the deposit ID)
            if (!Guid.TryParse(notification.ReferenceId, out var depositId))
            {
                _logger.LogError(
                    "Invalid deposit ID format in ReferenceId: {ReferenceId} for BillId: {BillId}",
                    notification.ReferenceId, notification.BillId);
                return;
            }

            var deposit = await _walletDepositRepository.GetByIdAsync(depositId, cancellationToken);
            if (deposit == null)
            {
                _logger.LogError(
                    "Deposit not found for ID: {DepositId}, BillId: {BillId}",
                    depositId, notification.BillId);
                return;
            }

            // Check if deposit is still pending
            if (deposit.Status != Domain.Enums.WalletDepositStatus.Pending)
            {
                _logger.LogWarning(
                    "Deposit {DepositId} is not in pending status (Status: {Status}), skipping wallet charge",
                    depositId, deposit.Status);
                return;
            }

            _logger.LogInformation(
                "Processing wallet deposit completion for DepositId: {DepositId}, User: {UserNationalNumber}, Amount: {AmountRials} rials",
                depositId, notification.UserNationalNumber, notification.TotalAmount.AmountRials);

            // Complete the deposit
            deposit.Complete();
            await _walletDepositRepository.UpdateAsync(deposit, cancellationToken: cancellationToken);

            _logger.LogInformation(
                "Deposit {DepositId} marked as completed, now charging wallet for user {UserNationalNumber}",
                depositId, notification.UserNationalNumber);

            // Charge the wallet using the ChargeWalletCommand
            var chargeCommand = new ChargeWalletCommand
            {
                UserNationalNumber = notification.UserNationalNumber,
                AmountRials = deposit.Amount.AmountRials,
                ReferenceId = deposit.Id.ToString(),
                ExternalReference = notification.LastGateway,
                Description = $"Wallet deposit completion via bill payment - DepositId: {depositId}, BillId: {notification.BillId}",
                Metadata = new Dictionary<string, string>
                {
                    ["DepositId"] = depositId.ToString(),
                    ["BillId"] = notification.BillId.ToString(),
                    ["BillNumber"] = notification.BillNumber,
                    ["PaymentCount"] = notification.PaymentCount.ToString(),
                    ["LastPaymentMethod"] = notification.LastPaymentMethod ?? string.Empty,
                    ["LastGateway"] = notification.LastGateway ?? string.Empty,
                    ["FullyPaidDate"] = notification.FullyPaidDate.ToString("O")
                }
            };

            var result = await _mediator.Send(chargeCommand, cancellationToken);

            if (result.IsSuccess)
            {
                _logger.LogInformation(
                    "Successfully completed deposit {DepositId} and charged wallet for user {UserNationalNumber}. TransactionId: {TransactionId}",
                    depositId, notification.UserNationalNumber, result.Data?.TransactionId);
            }
            else
            {
                _logger.LogError(
                    "Failed to charge wallet for user {UserNationalNumber} after completing deposit {DepositId}. Errors: {Errors}",
                    notification.UserNationalNumber, depositId, string.Join(", ", result.Errors));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error processing wallet charge for BillId: {BillId}, ReferenceId: {ReferenceId}",
                notification.BillId, notification.ReferenceId);
        }
    }

    /// <summary>
    /// Determines if the reference type indicates a wallet deposit bill
    /// </summary>
    private static bool IsWalletDepositBill(string referenceType)
    {
        return !string.IsNullOrEmpty(referenceType) && 
               string.Equals(referenceType, "WalletDeposit", StringComparison.OrdinalIgnoreCase);
    }
}
