using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Finance.Application.Commands.Wallets;
using Nezam.Refahi.Finance.Domain.Events;
using Nezam.Refahi.Finance.Domain.Repositories;

namespace Nezam.Refahi.Finance.Application.Consumers;

/// <summary>
/// مصرف‌کننده رویداد که کیف پول را شارژ می‌کند زمانی که پرداخت صورت حساب واریز کیف پول تکمیل می‌شود
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
    /// پردازش رویداد BillFullyPaidEvent برای صورت‌حساب‌های واریز کیف پول
    /// </summary>
    public async Task Handle(BillFullyPaidEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "پردازش رویداد پرداخت کامل صورت حساب - شناسه صورت حساب: {BillId}, کد پیگیری: {ReferenceId}, نوع مرجع: {ReferenceType}",
                notification.BillId, notification.ReferenceId, notification.ReferenceType);

            // Check if this is a wallet deposit bill by ReferenceType
            if (!IsWalletDepositBill(notification.ReferenceType))
            {
                _logger.LogDebug(
                    "صورت حساب {BillId} مربوط به واریز کیف پول نیست (نوع مرجع: {ReferenceType})، واریز کیف پول رد می‌شود",
                    notification.BillId, notification.ReferenceType);
                return;
            }

          

            var deposit = await _walletDepositRepository.GetByTrackingCodeAsync(notification.ReferenceId, cancellationToken);
            if (deposit == null)
            {
                _logger.LogError(
                    "واریز با کد پیگیری {ReferenceId} یافت نشد، شناسه صورت حساب: {BillId}",
                    notification.ReferenceId, notification.BillId);
                return;
            }

            // Check if deposit is still pending
            if (deposit.Status != Domain.Enums.WalletDepositStatus.Pending)
            {
                _logger.LogWarning(
                    "واریز {DepositId} در وضعیت انتظار نیست (وضعیت: {Status})، واریز کیف پول رد می‌شود",
                    notification.ReferenceId, deposit.Status); 
                return;
            }

            _logger.LogInformation(
                "پردازش تکمیل واریز کیف پول - شناسه واریز: {DepositId}, کاربر: {ExternalUserId}, مبلغ: {AmountRials} ریال",
                notification.ReferenceId, notification.ExternalUserId, notification.TotalAmount.AmountRials);

            // Complete the deposit
            deposit.Complete();
            await _walletDepositRepository.UpdateAsync(deposit, cancellationToken: cancellationToken);

            _logger.LogInformation(
                "واریز {DepositId} به عنوان تکمیل شده علامت‌گذاری شد، اکنون کیف پول کاربر {ExternalUserId} شارژ می‌شود",
                notification.ReferenceId, notification.ExternalUserId);

            // Charge the wallet using the ChargeWalletCommand
            var chargeCommand = new ChargeWalletCommand
            {
                ExternalUserId = notification.ExternalUserId,
                AmountRials = deposit.Amount.AmountRials,
                ReferenceId = deposit.Id.ToString(),
                ExternalReference = notification.LastGateway,
                Description = $"تکمیل واریز کیف پول از طریق پرداخت صورت حساب - کد پیگیری: {notification.ReferenceId}, شناسه صورت حساب: {notification.BillId}",
                Metadata = new Dictionary<string, string>
                {
                    ["DepositId"] = deposit.Id.ToString(),
                    ["TrackingCode"] = notification.ReferenceId,
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
                    "واریز {DepositId} با موفقیت تکمیل شد و کیف پول کاربر {ExternalUserId} شارژ شد. شناسه تراکنش: {TransactionId}",
                    notification.ReferenceId, notification.ExternalUserId, result.Data?.TransactionId);
            }
            else
            {
                _logger.LogError(
                    "خطا در شارژ کیف پول کاربر {ExternalUserId} پس از تکمیل واریز {DepositId}. خطاها: {Errors}",
                    notification.ExternalUserId, notification.ReferenceId, string.Join(", ", result.Errors));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "خطا در پردازش شارژ کیف پول - شناسه صورت حساب: {BillId}, کد پیگیری: {ReferenceId}",
                notification.BillId, notification.ReferenceId);
        }
    }

    /// <summary>
    /// تعیین می‌کند که آیا نوع مرجع نشان‌دهنده صورت حساب واریز کیف پول است
    /// </summary>
    private static bool IsWalletDepositBill(string referenceType)
    {
        return !string.IsNullOrEmpty(referenceType) && 
               string.Equals(referenceType, "WalletDeposit", StringComparison.OrdinalIgnoreCase);
    }
}
