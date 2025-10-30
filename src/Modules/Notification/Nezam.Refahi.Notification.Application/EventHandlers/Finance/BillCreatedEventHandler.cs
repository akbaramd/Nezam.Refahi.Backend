using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Contracts.Finance.v1.Messages;
using Nezam.Refahi.Finance.Contracts.IntegrationEvents;
using Nezam.Refahi.Notifications.Application.Features.Notifications.Commands.CreateNotification;
using Nezam.Refahi.Notifications.Application.Services;

namespace Nezam.Refahi.Notifications.Application.EventHandlers.Finance;

/// <summary>
/// Event handler for bill created events from Finance context
/// </summary>
public class BillCreatedEventHandler : INotificationHandler<BillCreatedEventMessage>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<BillCreatedEventHandler> _logger;
    
    public BillCreatedEventHandler(
        INotificationService notificationService,
        ILogger<BillCreatedEventHandler> logger)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task Handle(BillCreatedEventMessage notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling bill created event for user {UserId}, bill {BillNumber}, amount {Amount}", 
                notification.ExternalUserId, notification.BillNumber, notification.TotalAmountRials);
            
            // Create notification command
            var command = new CreateNotificationCommand
            {
                ExternalUserId = notification.ExternalUserId,
                Title = "فاکتور جدید صادر شد",
                Message = $"فاکتور {notification.BillNumber} با مبلغ {notification.TotalAmountRials:N0} تومان برای شما صادر شد.",
                Context = "Bill",
                Action = "BillCreated",
                Data = System.Text.Json.JsonSerializer.Serialize(new
                {
                    billId = notification.BillId,
                    billNumber = notification.BillNumber,
                    amount = notification.TotalAmountRials,
                    currency = notification.Currency,
                    issueDate = notification.IssueDate,
                    referenceType = notification.ReferenceType,
                    referenceId = notification.ReferenceId,
                    trackingCode = notification.TrackingCode
                }),
                ExpiresAt = DateTime.UtcNow.AddDays(30) // Expire 30 days after creation
            };
            
            // Create notification
            var result = await _notificationService.CreateNotificationAsync(command);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("Bill created notification sent successfully for user {UserId}, bill {BillNumber}", 
                    notification.ExternalUserId, notification.BillNumber);
            }
            else
            {
                _logger.LogError("Failed to send bill created notification for user {UserId}, bill {BillNumber}: {Errors}", 
                    notification.ExternalUserId, notification.BillNumber, string.Join(", ", result.Errors));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling bill created event for user {UserId}, bill {BillNumber}", 
                notification.ExternalUserId, notification.BillNumber);
        }
    }
}
