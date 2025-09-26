using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Finance.Domain.Events;
using Nezam.Refahi.Notifications.Application.Features.Notifications.Commands.CreateNotification;
using Nezam.Refahi.Notifications.Application.Services;

namespace Nezam.Refahi.Notifications.Application.EventHandlers.Finance;

/// <summary>
/// Event handler for wallet created events from Finance context
/// </summary>
public class WalletCreatedEventHandler : INotificationHandler<WalletCreatedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<WalletCreatedEventHandler> _logger;
    
    public WalletCreatedEventHandler(
        INotificationService notificationService,
        ILogger<WalletCreatedEventHandler> logger)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task Handle(WalletCreatedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling wallet created event for user {UserId}, wallet {WalletId}, initial balance {Balance}", 
                notification.ExternalUserId, notification.WalletId, notification.InitialBalance.AmountRials);
            
            // Create notification command
            var command = new CreateNotificationCommand
            {
                ExternalUserId = notification.ExternalUserId,
                Title = "کیف پول ایجاد شد",
                Message = $"کیف پول شما با موجودی اولیه {notification.InitialBalance.AmountRials:N0} تومان ایجاد شد.",
                Context = "Wallet",
                Action = "WalletCreated",
                Data = System.Text.Json.JsonSerializer.Serialize(new
                {
                    walletId = notification.WalletId,
                    initialBalance = notification.InitialBalance.AmountRials,
                    currency = "IRR",
                    createdAt = notification.CreatedAt
                }),
                ExpiresAt = DateTime.UtcNow.AddDays(30) // Keep for 30 days
            };
            
            // Create notification
            var result = await _notificationService.CreateNotificationAsync(command);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("Wallet created notification sent successfully for user {UserId}, wallet {WalletId}", 
                    notification.ExternalUserId, notification.WalletId);
            }
            else
            {
                _logger.LogError("Failed to send wallet created notification for user {UserId}, wallet {WalletId}: {Errors}", 
                    notification.ExternalUserId, notification.WalletId, string.Join(", ", result.Errors));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling wallet created event for user {UserId}, wallet {WalletId}", 
                notification.ExternalUserId, notification.WalletId);
        }
    }
}
