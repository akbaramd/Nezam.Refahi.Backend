using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Finance.Contracts.IntegrationEvents;
using Nezam.Refahi.Notifications.Application.Features.Notifications.Commands.CreateNotification;
using Nezam.Refahi.Notifications.Application.Services;

namespace Nezam.Refahi.Notifications.Application.EventHandlers.Finance;

/// <summary>
/// Event handler for payment completed events from Finance context
/// </summary>
public class PaymentCompletedEventHandler : INotificationHandler<PaymentCompletedIntegrationEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<PaymentCompletedEventHandler> _logger;
    
    public PaymentCompletedEventHandler(
        INotificationService notificationService,
        ILogger<PaymentCompletedEventHandler> logger)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task Handle(PaymentCompletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling payment completed event for user {UserId}, payment {PaymentId}, amount {Amount}", 
                notification.ExternalUserId, notification.PaymentId, notification.AmountRials);
            
            // Create notification command
            var command = new CreateNotificationCommand
            {
                ExternalUserId = notification.ExternalUserId,
                Title = "پرداخت موفق",
                Message = $"پرداخت شما با مبلغ {notification.AmountRials:N0} تومان با موفقیت انجام شد.",
                Context = "Payment",
                Action = "PaymentPaid",
                Data = System.Text.Json.JsonSerializer.Serialize(new
                {
                    paymentId = notification.PaymentId,
                    amountRials = notification.AmountRials,
                    currency = "IRR",
                    completedAt = notification.CompletedAt,
                    gatewayTransactionId = notification.GatewayTransactionId,
                    gatewayReference = notification.GatewayReference,
                    referenceId = notification.ReferenceId,
                    referenceType = notification.ReferenceType
                }),
                ExpiresAt = DateTime.UtcNow.AddDays(30) // Keep receipt for 30 days
            };
            
            // Create notification
            var result = await _notificationService.CreateNotificationAsync(command);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("Payment completed notification sent successfully for user {UserId}, payment {PaymentId}", 
                    notification.ExternalUserId, notification.PaymentId);
            }
            else
            {
                _logger.LogError("Failed to send payment completed notification for user {UserId}, payment {PaymentId}: {Errors}", 
                    notification.ExternalUserId, notification.PaymentId, string.Join(", ", result.Errors));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling payment completed event for user {UserId}, payment {PaymentId}", 
                notification.ExternalUserId, notification.PaymentId);
        }
    }
}
