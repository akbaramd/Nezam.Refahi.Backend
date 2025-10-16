using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Finance.Contracts.IntegrationEvents;
using Nezam.Refahi.Notifications.Application.Features.Notifications.Commands.CreateNotification;
using Nezam.Refahi.Notifications.Application.Services;

namespace Nezam.Refahi.Notifications.Application.EventHandlers.Finance;

/// <summary>
/// Event handler for bill fully paid events from Finance context
/// </summary>
public class BillFullyPaidEventHandler : INotificationHandler<BillFullyPaidIntegrationEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<BillFullyPaidEventHandler> _logger;
    
    public BillFullyPaidEventHandler(
        INotificationService notificationService,
        ILogger<BillFullyPaidEventHandler> logger)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task Handle(BillFullyPaidIntegrationEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling bill fully paid event for user {UserId}, bill {BillNumber}, total amount {TotalAmount}", 
                notification.ExternalUserId, notification.BillNumber, notification.PaidAmountRials);
            
            // Create notification command
            var command = new CreateNotificationCommand
            {
                ExternalUserId = notification.ExternalUserId,
                Title = "فاکتور کاملاً پرداخت شد",
                Message = $"فاکتور {notification.BillNumber} با مبلغ کل {notification.PaidAmountRials:N0} تومان کاملاً پرداخت شد.",
                Context = "Bill",
                Action = "BillFullyPaid",
                Data = System.Text.Json.JsonSerializer.Serialize(new
                {
                    billId = notification.BillId,
                    billNumber = notification.BillNumber,
                    paidAmountRials = notification.PaidAmountRials,
                    currency = "IRR",
                    paidAt = notification.PaidAt,
                    referenceId = notification.ReferenceId,
                    referenceType = notification.ReferenceType,
                    gatewayTransactionId = notification.GatewayTransactionId,
                    gatewayReference = notification.GatewayReference,
                    gateway = notification.Gateway
                }),
                ExpiresAt = DateTime.UtcNow.AddDays(90) // Keep for 90 days
            };
            
            // Create notification
            var result = await _notificationService.CreateNotificationAsync(command);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("Bill fully paid notification sent successfully for user {UserId}, bill {BillNumber}", 
                    notification.ExternalUserId, notification.BillNumber);
            }
            else
            {
                _logger.LogError("Failed to send bill fully paid notification for user {UserId}, bill {BillNumber}: {Errors}", 
                    notification.ExternalUserId, notification.BillNumber, string.Join(", ", result.Errors));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling bill fully paid event for user {UserId}, bill {BillNumber}", 
                notification.ExternalUserId, notification.BillNumber);
        }
    }
}
