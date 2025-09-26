using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Finance.Domain.Events;
using Nezam.Refahi.Notifications.Application.Features.Notifications.Commands.CreateNotification;
using Nezam.Refahi.Notifications.Application.Services;

namespace Nezam.Refahi.Notifications.Application.EventHandlers.Finance;

/// <summary>
/// Event handler for payment initiated events from Finance context
/// </summary>
public class PaymentInitiatedEventHandler : INotificationHandler<PaymentInitiatedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<PaymentInitiatedEventHandler> _logger;
    
    public PaymentInitiatedEventHandler(
        INotificationService notificationService,
        ILogger<PaymentInitiatedEventHandler> logger)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task Handle(PaymentInitiatedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling payment initiated event for user {UserId}, payment {PaymentId}, amount {Amount}", 
                notification.ExternalUserId, notification.PaymentId, notification.Amount.AmountRials);
            
            // Create notification command
            var command = new CreateNotificationCommand
            {
                ExternalUserId = notification.ExternalUserId,
                Title = "درخواست پرداخت ایجاد شد",
                Message = $"درخواست پرداخت با مبلغ {notification.Amount.AmountRials:N0} تومان برای فاکتور {notification.BillNumber} ایجاد شد.",
                Context = "Payment",
                Action = "PaymentInitiated",
                Data = System.Text.Json.JsonSerializer.Serialize(new
                {
                    paymentId = notification.PaymentId,
                    billId = notification.BillId,
                    billNumber = notification.BillNumber,
                    amount = notification.Amount.AmountRials,
                    currency = "IRR",
                    method = notification.Method.ToString(),
                    gateway = notification.Gateway?.ToString(),
                    trackingNumber = notification.TrackingNumber,
                    initiatedAt = notification.InitiatedAt,
                    expiryDate = notification.ExpiryDate,
                    referenceId = notification.ReferenceId,
                    referenceType = notification.ReferenceType
                }),
                ExpiresAt = notification.ExpiryDate ?? DateTime.UtcNow.AddHours(24) // Expire when payment expires or in 24 hours
            };
            
            // Create notification
            var result = await _notificationService.CreateNotificationAsync(command);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("Payment initiated notification sent successfully for user {UserId}, payment {PaymentId}", 
                    notification.ExternalUserId, notification.PaymentId);
            }
            else
            {
                _logger.LogError("Failed to send payment initiated notification for user {UserId}, payment {PaymentId}: {Errors}", 
                    notification.ExternalUserId, notification.PaymentId, string.Join(", ", result.Errors));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling payment initiated event for user {UserId}, payment {PaymentId}", 
                notification.ExternalUserId, notification.PaymentId);
        }
    }
}
