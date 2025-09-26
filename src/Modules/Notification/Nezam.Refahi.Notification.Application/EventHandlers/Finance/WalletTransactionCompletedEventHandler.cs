using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Finance.Domain.Events;
using Nezam.Refahi.Notifications.Application.Features.Notifications.Commands.CreateNotification;
using Nezam.Refahi.Notifications.Application.Services;

namespace Nezam.Refahi.Notifications.Application.EventHandlers.Finance;

/// <summary>
/// Event handler for wallet transaction completed events from Finance context
/// </summary>
public class WalletTransactionCompletedEventHandler : INotificationHandler<WalletTransactionCompletedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<WalletTransactionCompletedEventHandler> _logger;
    
    public WalletTransactionCompletedEventHandler(
        INotificationService notificationService,
        ILogger<WalletTransactionCompletedEventHandler> logger)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task Handle(WalletTransactionCompletedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling wallet transaction completed event for user {UserId}, wallet {WalletId}, transaction {TransactionId}, type {TransactionType}, amount {Amount}", 
                notification.ExternalUserId, notification.WalletId, notification.TransactionId, notification.TransactionType, notification.Amount.AmountRials);
            
            // Determine notification content based on transaction type
            var (title, message, action) = GetNotificationContent(notification);
            
            // Create notification command
            var command = new CreateNotificationCommand
            {
                ExternalUserId = notification.ExternalUserId,
                Title = title,
                Message = message,
                Context = "Wallet",
                Action = action,
                Data = System.Text.Json.JsonSerializer.Serialize(new
                {
                    transactionId = notification.TransactionId,
                    walletId = notification.WalletId,
                    transactionType = notification.TransactionType.ToString(),
                    amount = notification.Amount.AmountRials,
                    newBalance = notification.NewBalance.AmountRials,
                    currency = "IRR",
                    referenceId = notification.ReferenceId,
                    description = notification.Description,
                    completedAt = notification.CompletedAt
                }),
                ExpiresAt = DateTime.UtcNow.AddDays(30) // Keep for 30 days
            };
            
            // Create notification
            var result = await _notificationService.CreateNotificationAsync(command);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("Wallet transaction completed notification sent successfully for user {UserId}, transaction {TransactionId}", 
                    notification.ExternalUserId, notification.TransactionId);
            }
            else
            {
                _logger.LogError("Failed to send wallet transaction completed notification for user {UserId}, transaction {TransactionId}: {Errors}", 
                    notification.ExternalUserId, notification.TransactionId, string.Join(", ", result.Errors));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling wallet transaction completed event for user {UserId}, transaction {TransactionId}", 
                notification.ExternalUserId, notification.TransactionId);
        }
    }
    
    private static (string title, string message, string action) GetNotificationContent(WalletTransactionCompletedEvent notification)
    {
        return notification.TransactionType.ToString() switch
        {
            "Deposit" => (
                "واریز موفق",
                $"واریز {notification.Amount.AmountRials:N0} تومان با موفقیت انجام شد. موجودی جدید: {notification.NewBalance.AmountRials:N0} تومان",
                "WalletDepositCompleted"
            ),
            "Withdrawal" => (
                "برداشت موفق",
                $"برداشت {notification.Amount.AmountRials:N0} تومان با موفقیت انجام شد. موجودی جدید: {notification.NewBalance.AmountRials:N0} تومان",
                "WalletWithdrawalCompleted"
            ),
            "TransferIn" => (
                "انتقال موفق",
                $"انتقال {notification.Amount.AmountRials:N0} تومان با موفقیت انجام شد. موجودی جدید: {notification.NewBalance.AmountRials:N0} تومان",
                "WalletTransferInCompleted"
            ),
            "TransferOut" => (
                "انتقال موفق",
                $"انتقال {notification.Amount.AmountRials:N0} تومان با موفقیت انجام شد. موجودی جدید: {notification.NewBalance.AmountRials:N0} تومان",
                "WalletTransferOutCompleted"
            ),
            "Refund" => (
                "بازگشت وجه موفق",
                $"بازگشت {notification.Amount.AmountRials:N0} تومان با موفقیت انجام شد. موجودی جدید: {notification.NewBalance.AmountRials:N0} تومان",
                "WalletRefundCompleted"
            ),
            "Reward" => (
                "پاداش دریافت شد",
                $"پاداش {notification.Amount.AmountRials:N0} تومان با موفقیت به کیف پول شما اضافه شد. موجودی جدید: {notification.NewBalance.AmountRials:N0} تومان",
                "WalletRewardCompleted"
            ),
            "Penalty" => (
                "جریمه کسر شد",
                $"جریمه {notification.Amount.AmountRials:N0} تومان از کیف پول شما کسر شد. موجودی جدید: {notification.NewBalance.AmountRials:N0} تومان",
                "WalletPenaltyCompleted"
            ),
            _ => (
                "تراکنش تکمیل شد",
                $"تراکنش {notification.Amount.AmountRials:N0} تومان با موفقیت تکمیل شد. موجودی جدید: {notification.NewBalance.AmountRials:N0} تومان",
                "WalletTransactionCompleted"
            )
        };
    }
}
