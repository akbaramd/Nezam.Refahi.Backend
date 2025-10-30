using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Finance.Application.Commands.Wallets;
using Nezam.Refahi.Finance.Contracts.IntegrationEvents;
using Nezam.Refahi.Finance.Domain.Events;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application;
using Nezam.Refahi.Shared.Application.Common.Interfaces;

namespace Nezam.Refahi.Finance.Application.Consumers;

/// <summary>
/// مصرف‌کننده رویداد که کیف پول را شارژ می‌کند زمانی که پرداخت صورت حساب واریز کیف پول تکمیل می‌شود
/// </summary>
public class WalletChargePaymentCompletedConsumer : INotificationHandler<BillFullyPaidCompletedIntegrationEvent>
{
    private readonly IMediator _mediator;
    private readonly IBillRepository _billRepository;
    private readonly IWalletDepositRepository _walletDepositRepository;
    private readonly IOutboxPublisher _outboxPublisher;
    private readonly ILogger<WalletChargePaymentCompletedConsumer> _logger;

    public WalletChargePaymentCompletedConsumer(
        IMediator mediator,
        IBillRepository billRepository,
        IWalletDepositRepository walletDepositRepository,
        IOutboxPublisher outboxPublisher,
        ILogger<WalletChargePaymentCompletedConsumer> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _walletDepositRepository = walletDepositRepository ?? throw new ArgumentNullException(nameof(walletDepositRepository));
        _outboxPublisher = outboxPublisher ?? throw new ArgumentNullException(nameof(outboxPublisher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// پردازش رویداد BillFullyPaidCompletedIntegrationEvent برای صورت‌حساب‌های واریز کیف پول
    /// </summary>
    public async Task Handle(BillFullyPaidCompletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "پردازش رویداد پرداخت کامل صورت حساب - شناسه صورت حساب: {BillId}, کد پیگیری: {ReferenceId}, نوع مرجع: {ReferenceType}",
                notification.BillId, notification.ReferenceId, notification.ReferenceType);

            // IDEMPOTENCY CHECK: Prevent duplicate processing
            var idempotencyKey = $"wallet_charge_{notification.BillId}_{notification.PaymentId}";
            if (await IsAlreadyProcessedAsync(idempotencyKey, cancellationToken))
            {
                _logger.LogWarning(
                    "رویداد پرداخت کامل صورت حساب {BillId} قبلاً پردازش شده است (Idempotency Key: {IdempotencyKey}). پردازش رد می‌شود.",
                    notification.BillId, idempotencyKey);
                return;
            }

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
                
                // Publish failure event for manual intervention
                await PublishWalletChargeFailedEvent(notification, "DEPOSIT_NOT_FOUND", 
                    $"واریز با کد پیگیری {notification.ReferenceId} یافت نشد", 0, cancellationToken);
                return;
            }

            // Check if deposit is still pending
            if (deposit.Status != Domain.Enums.WalletDepositStatus.Pending)
            {
                _logger.LogWarning(
                    "واریز {DepositId} در وضعیت انتظار نیست (وضعیت: {Status})، واریز کیف پول رد می‌شود",
                    notification.ReferenceId, deposit.Status);
                
                // Publish failure event for manual intervention
                await PublishWalletChargeFailedEvent(notification, "DEPOSIT_NOT_PENDING", 
                    $"واریز در وضعیت انتظار نیست (وضعیت: {deposit.Status})", 0, cancellationToken);
                return;
            }

            _logger.LogInformation(
                "پردازش تکمیل واریز کیف پول - شناسه واریز: {DepositId}, کاربر: {ExternalUserId}, مبلغ: {AmountRials} ریال",
                notification.ReferenceId, notification.ExternalUserId, notification.PaidAmountRials);

            // Publish wallet charge requested event with idempotency
            await PublishWalletChargeRequestedEvent(notification, deposit, cancellationToken);

            // Complete the deposit
            deposit.Complete();
            await _walletDepositRepository.UpdateAsync(deposit, cancellationToken: cancellationToken);

            _logger.LogInformation(
                "واریز {DepositId} به عنوان تکمیل شده علامت‌گذاری شد، اکنون کیف پول کاربر {ExternalUserId} شارژ می‌شود",
                notification.ReferenceId, notification.ExternalUserId);

            // Charge the wallet using the ChargeWalletCommand with retry mechanism
            var maxRetries = 3;
            var retryCount = 0;
            Exception? lastException = null;

            while (retryCount < maxRetries)
            {
                try
                {
                    var chargeCommand = new ChargeWalletCommand
                    {
                        ExternalUserId = notification.ExternalUserId,
                        AmountRials = deposit.Amount.AmountRials,
                        ReferenceId = deposit.Id.ToString(),
                        ExternalReference = notification.GatewayTransactionId,
                        Description = $"تکمیل واریز کیف پول از طریق پرداخت صورت حساب - کد پیگیری: {notification.ReferenceId}, شناسه صورت حساب: {notification.BillId}",
                        Metadata = new Dictionary<string, string>
                        {
                            ["DepositId"] = deposit.Id.ToString(),
                            ["TrackingCode"] = notification.ReferenceId,
                            ["BillId"] = notification.BillId.ToString(),
                            ["BillNumber"] = notification.BillNumber,
                            ["PaymentCount"] = notification.Metadata.ContainsKey("PaymentCount") ? notification.Metadata["PaymentCount"] : "1",
                            ["LastPaymentMethod"] = notification.Metadata.ContainsKey("LastPaymentMethod") ? notification.Metadata["LastPaymentMethod"] : string.Empty,
                            ["LastGateway"] = notification.Metadata.ContainsKey("LastGateway") ? notification.Metadata["LastGateway"] : string.Empty,
                            ["FullyPaidDate"] = notification.PaidAt.ToString("O"),
                            ["RetryCount"] = retryCount.ToString()
                        }
                    };

                    var result = await _mediator.Send(chargeCommand, cancellationToken);

                    if (result.IsSuccess)
                    {
                        _logger.LogInformation(
                            "واریز {DepositId} با موفقیت تکمیل شد و کیف پول کاربر {ExternalUserId} شارژ شد. شناسه تراکنش: {TransactionId}",
                            notification.ReferenceId, notification.ExternalUserId, result.Data?.TransactionId);

                        // Publish success event
                        await PublishWalletChargeCompletedEvent(notification, deposit, result.Data?.TransactionId ?? Guid.Empty, cancellationToken);
                        return;
                    }
                    else
                    {
                        lastException = new InvalidOperationException($"Wallet charge failed: {string.Join(", ", result.Errors)}");
                        retryCount++;
                        
                        if (retryCount < maxRetries)
                        {
                            var delay = TimeSpan.FromSeconds(Math.Pow(2, retryCount)); // Exponential backoff
                            _logger.LogWarning(
                                "خطا در شارژ کیف پول کاربر {ExternalUserId} پس از تکمیل واریز {DepositId}. تلاش مجدد {RetryCount}/{MaxRetries} در {Delay} ثانیه. خطاها: {Errors}",
                                notification.ExternalUserId, notification.ReferenceId, retryCount, maxRetries, delay.TotalSeconds, string.Join(", ", result.Errors));
                            
                            await Task.Delay(delay, cancellationToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    retryCount++;
                    
                    if (retryCount < maxRetries)
                    {
                        var delay = TimeSpan.FromSeconds(Math.Pow(2, retryCount)); // Exponential backoff
                        _logger.LogWarning(ex,
                            "خطا در شارژ کیف پول کاربر {ExternalUserId} پس از تکمیل واریز {DepositId}. تلاش مجدد {RetryCount}/{MaxRetries} در {Delay} ثانیه",
                            notification.ExternalUserId, notification.ReferenceId, retryCount, maxRetries, delay.TotalSeconds);
                        
                        await Task.Delay(delay, cancellationToken);
                    }
                }
            }

            // All retries failed - publish failure event for manual intervention
            _logger.LogError(lastException,
                "تمام تلاش‌های شارژ کیف پول کاربر {ExternalUserId} پس از تکمیل واریز {DepositId} ناموفق بود. نیاز به مداخله دستی",
                notification.ExternalUserId, notification.ReferenceId);

            await PublishWalletChargeFailedEvent(notification, "MAX_RETRIES_EXCEEDED", 
                $"تمام تلاش‌های شارژ کیف پول ناموفق بود: {lastException?.Message}", maxRetries, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "خطا در پردازش شارژ کیف پول - شناسه صورت حساب: {BillId}, کد پیگیری: {ReferenceId}",
                notification.BillId, notification.ReferenceId);

            // Publish failure event for manual intervention
            await PublishWalletChargeFailedEvent(notification, "UNEXPECTED_ERROR", 
                $"خطای غیرمنتظره در پردازش شارژ کیف پول: {ex.Message}", 0, CancellationToken.None);
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

    /// <summary>
    /// بررسی می‌کند که آیا رویداد قبلاً پردازش شده است یا نه
    /// </summary>
    private async Task<bool> IsAlreadyProcessedAsync(string idempotencyKey, CancellationToken cancellationToken)
    {
        try
        {
            // Check if we have already processed this event by looking at Outbox messages
            // This prevents duplicate processing even if the same event is received multiple times
            var existingMessage = await _outboxPublisher.GetByIdempotencyKeyAsync(idempotencyKey, cancellationToken);
            return existingMessage != null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "خطا در بررسی Idempotency Key {IdempotencyKey}. ادامه پردازش...", idempotencyKey);
            return false; // Continue processing if check fails
        }
    }

    /// <summary>
    /// انتشار رویداد درخواست شارژ کیف پول
    /// </summary>
    private async Task PublishWalletChargeRequestedEvent(BillFullyPaidCompletedIntegrationEvent notification, Domain.Entities.WalletDeposit deposit, CancellationToken cancellationToken)
    {
        try
        {
            var walletChargeRequestedEvent = new WalletChargeRequestedIntegrationEvent
            {
                BillId = notification.BillId,
                BillNumber = notification.BillNumber,
                ReferenceId = notification.ReferenceId,
                ReferenceType = notification.ReferenceType,
                ExternalUserId = notification.ExternalUserId,
                UserFullName = notification.UserFullName,
                AmountRials = deposit.Amount.AmountRials,
                PaidAt = notification.PaidAt,
                PaymentId = notification.PaymentId,
                GatewayTransactionId = notification.GatewayTransactionId,
                GatewayReference = notification.GatewayReference,
                Gateway = notification.Gateway,
                PaymentMethod = notification.Metadata.ContainsKey("PaymentMethod") ? notification.Metadata["PaymentMethod"] : "Unknown",
                Metadata = new Dictionary<string, string>
                {
                    ["DepositId"] = deposit.Id.ToString(),
                    ["TrackingCode"] = notification.ReferenceId,
                    ["BillId"] = notification.BillId.ToString(),
                    ["BillNumber"] = notification.BillNumber,
                    ["PaymentCount"] = notification.Metadata.ContainsKey("PaymentCount") ? notification.Metadata["PaymentCount"] : "1",
                    ["LastPaymentMethod"] = notification.Metadata.ContainsKey("LastPaymentMethod") ? notification.Metadata["LastPaymentMethod"] : string.Empty,
                    ["LastGateway"] = notification.Metadata.ContainsKey("LastGateway") ? notification.Metadata["LastGateway"] : string.Empty,
                    ["FullyPaidDate"] = notification.PaidAt.ToString("O")
                }
            };

            var requestedIdempotencyKey = $"wallet_charge_requested_{notification.BillId}_{notification.PaymentId}";
            await _outboxPublisher.PublishAsync(
                walletChargeRequestedEvent, 
                aggregateId: notification.BillId,
                correlationId: notification.GatewayTransactionId,
                idempotencyKey: requestedIdempotencyKey,
                cancellationToken);
            _logger.LogInformation("رویداد درخواست شارژ کیف پول منتشر شد - شناسه صورت حساب: {BillId}", notification.BillId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در انتشار رویداد درخواست شارژ کیف پول - شناسه صورت حساب: {BillId}", notification.BillId);
        }
    }

    /// <summary>
    /// انتشار رویداد تکمیل شارژ کیف پول
    /// </summary>
    private async Task PublishWalletChargeCompletedEvent(BillFullyPaidCompletedIntegrationEvent notification, Domain.Entities.WalletDeposit deposit, Guid walletTransactionId, CancellationToken cancellationToken)
    {
        try
        {
            var walletChargeCompletedEvent = new WalletChargeCompletedIntegrationEvent
            {
                BillId = notification.BillId,
                BillNumber = notification.BillNumber,
                ReferenceId = notification.ReferenceId,
                ReferenceType = notification.ReferenceType,
                ExternalUserId = notification.ExternalUserId,
                UserFullName = notification.UserFullName,
                AmountRials = deposit.Amount.AmountRials,
                ChargedAt = DateTime.UtcNow,
                WalletTransactionId = walletTransactionId,
                NewWalletBalance = 0, // Will be filled by wallet service
                PaymentId = notification.PaymentId,
                GatewayTransactionId = notification.GatewayTransactionId,
                GatewayReference = notification.GatewayReference,
                Gateway = notification.Gateway,
                PaymentMethod = notification.Metadata.ContainsKey("PaymentMethod") ? notification.Metadata["PaymentMethod"] : "Unknown",
                Metadata = new Dictionary<string, string>
                {
                    ["DepositId"] = deposit.Id.ToString(),
                    ["TrackingCode"] = notification.ReferenceId,
                    ["BillId"] = notification.BillId.ToString(),
                    ["BillNumber"] = notification.BillNumber,
                    ["PaymentCount"] = notification.Metadata.ContainsKey("PaymentCount") ? notification.Metadata["PaymentCount"] : "1",
                    ["LastPaymentMethod"] = notification.Metadata.ContainsKey("LastPaymentMethod") ? notification.Metadata["LastPaymentMethod"] : string.Empty,
                    ["LastGateway"] = notification.Metadata.ContainsKey("LastGateway") ? notification.Metadata["LastGateway"] : string.Empty,
                    ["FullyPaidDate"] = notification.PaidAt.ToString("O"),
                    ["WalletTransactionId"] = walletTransactionId.ToString()
                }
            };

            var completedIdempotencyKey = $"wallet_charge_completed_{notification.BillId}_{notification.PaymentId}";
            await _outboxPublisher.PublishAsync(
                walletChargeCompletedEvent, 
                aggregateId: notification.BillId,
                correlationId: notification.GatewayTransactionId,
                idempotencyKey: completedIdempotencyKey,
                cancellationToken);
            _logger.LogInformation("رویداد تکمیل شارژ کیف پول منتشر شد - شناسه صورت حساب: {BillId}, شناسه تراکنش کیف پول: {TransactionId}", 
                notification.BillId, walletTransactionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در انتشار رویداد تکمیل شارژ کیف پول - شناسه صورت حساب: {BillId}", notification.BillId);
        }
    }

    /// <summary>
    /// انتشار رویداد شکست شارژ کیف پول
    /// </summary>
    private async Task PublishWalletChargeFailedEvent(BillFullyPaidCompletedIntegrationEvent notification, string errorCode, string errorMessage, int retryCount, CancellationToken cancellationToken)
    {
        try
        {
            var walletChargeFailedEvent = new WalletChargeFailedIntegrationEvent
            {
                BillId = notification.BillId,
                BillNumber = notification.BillNumber,
                ReferenceId = notification.ReferenceId,
                ReferenceType = notification.ReferenceType,
                ExternalUserId = notification.ExternalUserId,
                UserFullName = notification.UserFullName,
                AmountRials = notification.PaidAmountRials,
                FailedAt = DateTime.UtcNow,
                PaymentId = notification.PaymentId,
                GatewayTransactionId = notification.GatewayTransactionId,
                GatewayReference = notification.GatewayReference,
                Gateway = notification.Gateway,
                PaymentMethod = notification.Metadata.ContainsKey("PaymentMethod") ? notification.Metadata["PaymentMethod"] : "Unknown",
                ErrorCode = errorCode,
                ErrorMessage = errorMessage,
                RetryCount = retryCount,
                NextRetryAt = retryCount < 3 ? DateTime.UtcNow.AddSeconds(Math.Pow(2, retryCount + 1)) : null,
                Metadata = new Dictionary<string, string>
                {
                    ["BillId"] = notification.BillId.ToString(),
                    ["BillNumber"] = notification.BillNumber,
                    ["ReferenceId"] = notification.ReferenceId,
                    ["ReferenceType"] = notification.ReferenceType,
                    ["PaymentCount"] = notification.Metadata.ContainsKey("PaymentCount") ? notification.Metadata["PaymentCount"] : "1",
                    ["LastPaymentMethod"] = notification.Metadata.ContainsKey("LastPaymentMethod") ? notification.Metadata["LastPaymentMethod"] : string.Empty,
                    ["LastGateway"] = notification.Metadata.ContainsKey("LastGateway") ? notification.Metadata["LastGateway"] : string.Empty,
                    ["FullyPaidDate"] = notification.PaidAt.ToString("O"),
                    ["ErrorCode"] = errorCode,
                    ["RetryCount"] = retryCount.ToString(),
                    ["RequiresManualIntervention"] = (retryCount >= 3).ToString()
                }
            };

            var failedIdempotencyKey = $"wallet_charge_failed_{notification.BillId}_{notification.PaymentId}_{errorCode}";
            await _outboxPublisher.PublishAsync(
                walletChargeFailedEvent, 
                aggregateId: notification.BillId,
                correlationId: notification.GatewayTransactionId,
                idempotencyKey: failedIdempotencyKey,
                cancellationToken);
            _logger.LogWarning("رویداد شکست شارژ کیف پول منتشر شد - شناسه صورت حساب: {BillId}, کد خطا: {ErrorCode}, تعداد تلاش: {RetryCount}", 
                notification.BillId, errorCode, retryCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در انتشار رویداد شکست شارژ کیف پول - شناسه صورت حساب: {BillId}", notification.BillId);
        }
    }
}
