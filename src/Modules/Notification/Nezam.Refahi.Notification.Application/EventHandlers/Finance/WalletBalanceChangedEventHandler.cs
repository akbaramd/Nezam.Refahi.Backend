using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Finance.Domain.Events;
using Nezam.Refahi.Notifications.Application.Features.Notifications.Commands.CreateNotification;
using Nezam.Refahi.Notifications.Application.Services;

namespace Nezam.Refahi.Notifications.Application.EventHandlers.Finance;

/// <summary>
/// Event handler for wallet balance changed events from Finance context
/// </summary>
public class WalletBalanceChangedEventHandler : INotificationHandler<WalletBalanceChangedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<WalletBalanceChangedEventHandler> _logger;
    
    public WalletBalanceChangedEventHandler(
        INotificationService notificationService,
        ILogger<WalletBalanceChangedEventHandler> logger)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task Handle(WalletBalanceChangedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling wallet balance changed event for user {UserId}, wallet {WalletId}, transaction type {TransactionType}, amount {Amount}", 
                notification.ExternalUserId, notification.WalletId, notification.TransactionType, notification.TransactionAmount.AmountRials);
            
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
                    walletId = notification.WalletId,
                    transactionType = notification.TransactionType.ToString(),
                    previousBalance = notification.PreviousBalance.AmountRials,
                    newBalance = notification.NewBalance.AmountRials,
                    transactionAmount = notification.TransactionAmount.AmountRials,
                    currency = "IRR",
                    referenceId = notification.ReferenceId,
                    changedAt = notification.ChangedAt
                }),
                ExpiresAt = DateTime.UtcNow.AddDays(30) // Keep for 30 days
            };
            
            // Create notification
            var result = await _notificationService.CreateNotificationAsync(command);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("Wallet balance changed notification sent successfully for user {UserId}, wallet {WalletId}", 
                    notification.ExternalUserId, notification.WalletId);
            }
            else
            {
                _logger.LogError("Failed to send wallet balance changed notification for user {UserId}, wallet {WalletId}: {Errors}", 
                    notification.ExternalUserId, notification.WalletId, string.Join(", ", result.Errors));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling wallet balance changed event for user {UserId}, wallet {WalletId}", 
                notification.ExternalUserId, notification.WalletId);
        }
    }
    
    private static (string title, string message, string action) GetNotificationContent(WalletBalanceChangedEvent notification)
    {
        return notification.TransactionType.ToString() switch
        {
            "Deposit" => (
                "واریز به کیف پول",
                $"مبلغ {notification.TransactionAmount.AmountRials:N0} تومان به کیف پول شما واریز شد. موجودی جدید: {notification.NewBalance.AmountRials:N0} تومان",
                "WalletDeposit"
            ),
            "Withdrawal" => (
                "برداشت از کیف پول",
                $"مبلغ {notification.TransactionAmount.AmountRials:N0} تومان از کیف پول شما برداشت شد. موجودی جدید: {notification.NewBalance.AmountRials:N0} تومان",
                "WalletWithdrawal"
            ),
            "TransferIn" => (
                "انتقال به کیف پول",
                $"مبلغ {notification.TransactionAmount.AmountRials:N0} تومان به کیف پول شما انتقال یافت. موجودی جدید: {notification.NewBalance.AmountRials:N0} تومان",
                "WalletTransferIn"
            ),
            "TransferOut" => (
                "انتقال از کیف پول",
                $"مبلغ {notification.TransactionAmount.AmountRials:N0} تومان از کیف پول شما انتقال یافت. موجودی جدید: {notification.NewBalance.AmountRials:N0} تومان",
                "WalletTransferOut"
            ),
            "Refund" => (
                "بازگشت وجه",
                $"مبلغ {notification.TransactionAmount.AmountRials:N0} تومان به کیف پول شما بازگشت یافت. موجودی جدید: {notification.NewBalance.AmountRials:N0} تومان",
                "WalletRefund"
            ),
            "Reward" => (
                "پاداش دریافت شد",
                $"مبلغ {notification.TransactionAmount.AmountRials:N0} تومان به عنوان پاداش به کیف پول شما اضافه شد. موجودی جدید: {notification.NewBalance.AmountRials:N0} تومان",
                "WalletReward"
            ),
            "Penalty" => (
                "جریمه کسر شد",
                $"مبلغ {notification.TransactionAmount.AmountRials:N0} تومان به عنوان جریمه از کیف پول شما کسر شد. موجودی جدید: {notification.NewBalance.AmountRials:N0} تومان",
                "WalletPenalty"
            ),
            _ => (
                "تغییر موجودی کیف پول",
                $"موجودی کیف پول شما تغییر کرد. موجودی جدید: {notification.NewBalance.AmountRials:N0} تومان",
                "WalletBalanceChanged"
            )
        };
    }
}
