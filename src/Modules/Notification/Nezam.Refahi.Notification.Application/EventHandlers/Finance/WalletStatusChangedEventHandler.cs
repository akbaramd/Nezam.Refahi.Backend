using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Finance.Domain.Events;
using Nezam.Refahi.Notifications.Application.Features.Notifications.Commands.CreateNotification;
using Nezam.Refahi.Notifications.Application.Services;

namespace Nezam.Refahi.Notifications.Application.EventHandlers.Finance;

/// <summary>
/// Event handler for wallet status changed events from Finance context
/// </summary>
public class WalletStatusChangedEventHandler : INotificationHandler<WalletStatusChangedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<WalletStatusChangedEventHandler> _logger;
    
    public WalletStatusChangedEventHandler(
        INotificationService notificationService,
        ILogger<WalletStatusChangedEventHandler> logger)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task Handle(WalletStatusChangedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling wallet status changed event for user {UserId}, wallet {WalletId}, status {PreviousStatus} -> {NewStatus}", 
                notification.ExternalUserId, notification.WalletId, notification.PreviousStatus, notification.NewStatus);
            
            // Determine notification content based on status change
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
                    previousStatus = notification.PreviousStatus.ToString(),
                    newStatus = notification.NewStatus.ToString(),
                    reason = notification.Reason,
                    changedAt = notification.ChangedAt
                }),
                ExpiresAt = DateTime.UtcNow.AddDays(30) // Keep for 30 days
            };
            
            // Create notification
            var result = await _notificationService.CreateNotificationAsync(command);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("Wallet status changed notification sent successfully for user {UserId}, wallet {WalletId}", 
                    notification.ExternalUserId, notification.WalletId);
            }
            else
            {
                _logger.LogError("Failed to send wallet status changed notification for user {UserId}, wallet {WalletId}: {Errors}", 
                    notification.ExternalUserId, notification.WalletId, string.Join(", ", result.Errors));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling wallet status changed event for user {UserId}, wallet {WalletId}", 
                notification.ExternalUserId, notification.WalletId);
        }
    }
    
    private static (string title, string message, string action) GetNotificationContent(WalletStatusChangedEvent notification)
    {
        var statusChange = $"{notification.PreviousStatus} -> {notification.NewStatus}";
        
        return statusChange switch
        {
            "Active -> Suspended" => (
                "کیف پول معلق شد",
                $"کیف پول شما به دلیل {notification.Reason ?? "دلایل امنیتی"} معلق شد. لطفاً با پشتیبانی تماس بگیرید.",
                "WalletSuspended"
            ),
            "Suspended -> Active" => (
                "کیف پول فعال شد",
                $"کیف پول شما مجدداً فعال شد و می‌توانید از آن استفاده کنید.",
                "WalletActivated"
            ),
            "Active -> Blocked" => (
                "کیف پول مسدود شد",
                $"کیف پول شما به دلیل {notification.Reason ?? "نقض قوانین"} مسدود شد. لطفاً با پشتیبانی تماس بگیرید.",
                "WalletBlocked"
            ),
            "Blocked -> Active" => (
                "کیف پول آزاد شد",
                $"کیف پول شما آزاد شد و می‌توانید از آن استفاده کنید.",
                "WalletUnblocked"
            ),
            "Active -> Inactive" => (
                "کیف پول غیرفعال شد",
                $"کیف پول شما غیرفعال شد. برای فعال‌سازی مجدد با پشتیبانی تماس بگیرید.",
                "WalletDeactivated"
            ),
            "Inactive -> Active" => (
                "کیف پول فعال شد",
                $"کیف پول شما فعال شد و می‌توانید از آن استفاده کنید.",
                "WalletActivated"
            ),
            _ => (
                "تغییر وضعیت کیف پول",
                $"وضعیت کیف پول شما از {notification.PreviousStatus} به {notification.NewStatus} تغییر یافت.",
                "WalletStatusChanged"
            )
        };
    }
}
