using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Finance.Domain.Events;
using Nezam.Refahi.Notifications.Application.Features.Notifications.Commands.CreateNotification;
using Nezam.Refahi.Notifications.Application.Services;

namespace Nezam.Refahi.Notifications.Application.EventHandlers.Finance;

/// <summary>
/// Event handler for bill overdue events from Finance context
/// </summary>
public class BillOverdueEventHandler : INotificationHandler<BillOverdueEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<BillOverdueEventHandler> _logger;
    
    public BillOverdueEventHandler(
        INotificationService notificationService,
        ILogger<BillOverdueEventHandler> logger)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task Handle(BillOverdueEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling bill overdue event for user {UserId}, bill {BillNumber}, days overdue {DaysOverdue}", 
                notification.ExternalUserId, notification.BillNumber, notification.DaysOverdue);
            
            // Create notification command
            var command = new CreateNotificationCommand
            {
                ExternalUserId = notification.ExternalUserId,
                Title = "فاکتور منقضی شد",
                Message = $"فاکتور {notification.BillNumber} با مبلغ {notification.RemainingAmount.AmountRials:N0} تومان منقضی شده است. لطفاً هرچه سریع‌تر پرداخت کنید.",
                Context = "Bill",
                Action = "BillOverdue",
                Data = System.Text.Json.JsonSerializer.Serialize(new
                {
                    billId = notification.BillId,
                    billNumber = notification.BillNumber,
                    totalAmount = notification.TotalAmount.AmountRials,
                    remainingAmount = notification.RemainingAmount.AmountRials,
                    currency = "IRR",
                    dueDate = notification.DueDate,
                    overdueDate = notification.OverdueDate,
                    daysOverdue = notification.DaysOverdue,
                    referenceId = notification.ReferenceId,
                    referenceType = notification.ReferenceType
                }),
                ExpiresAt = DateTime.UtcNow.AddDays(7) // Urgent - expire in 7 days
            };
            
            // Create notification
            var result = await _notificationService.CreateNotificationAsync(command);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("Bill overdue notification sent successfully for user {UserId}, bill {BillNumber}", 
                    notification.ExternalUserId, notification.BillNumber);
            }
            else
            {
                _logger.LogError("Failed to send bill overdue notification for user {UserId}, bill {BillNumber}: {Errors}", 
                    notification.ExternalUserId, notification.BillNumber, string.Join(", ", result.Errors));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling bill overdue event for user {UserId}, bill {BillNumber}", 
                notification.ExternalUserId, notification.BillNumber);
        }
    }
}
