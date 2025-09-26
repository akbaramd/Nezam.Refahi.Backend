using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Notifications.Application.Features.Notifications.Commands.CreateNotification;
using Nezam.Refahi.Notifications.Application.Services;

namespace Nezam.Refahi.Notifications.Application.EventHandlers;

/// <summary>
/// Event handler for notification saved events
/// This handler can be used to create follow-up notifications or perform additional actions
/// when a notification is successfully saved
/// </summary>
public class NotificationSavedEventHandler : INotificationHandler<NotificationSavedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationSavedEventHandler> _logger;
    
    public NotificationSavedEventHandler(
        INotificationService notificationService,
        ILogger<NotificationSavedEventHandler> logger)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task Handle(NotificationSavedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling notification saved event for notification {NotificationId}, user {UserId}, context {Context}, action {Action}", 
                notification.NotificationId, notification.ExternalUserId, notification.Context, notification.Action);
            
            // Create follow-up notification if needed based on context and action
            if (ShouldCreateFollowUpNotification(notification))
            {
                var followUpCommand = CreateFollowUpNotification(notification);
                
                var result = await _notificationService.CreateNotificationAsync(followUpCommand);
                
                if (result.IsSuccess)
                {
                    _logger.LogInformation("Follow-up notification created successfully for notification {NotificationId}", notification.NotificationId);
                }
                else
                {
                    _logger.LogError("Failed to create follow-up notification for notification {NotificationId}: {Errors}", 
                        notification.NotificationId, string.Join(", ", result.Errors));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling notification saved event for notification {NotificationId}", notification.NotificationId);
        }
    }
    
    private static bool ShouldCreateFollowUpNotification(NotificationSavedEvent notification)
    {
        // Define rules for when to create follow-up notifications
        return notification.Context switch
        {
            "Payment" when notification.Action == "PaymentInitiated" => true, // Send reminder after 1 hour
            "Bill" when notification.Action == "BillOverdue" => true, // Send urgent reminder
            "TourReservation" when notification.Action == "ReservationCreated" => true, // Send confirmation reminder
            _ => false
        };
    }
    
    private static CreateNotificationCommand CreateFollowUpNotification(NotificationSavedEvent notification)
    {
        return notification.Context switch
        {
            "Payment" when notification.Action == "PaymentInitiated" => new CreateNotificationCommand
            {
                ExternalUserId = notification.ExternalUserId,
                Title = "یادآوری پرداخت",
                Message = "لطفاً پرداخت خود را تکمیل کنید. در صورت عدم پرداخت، رزرو شما لغو خواهد شد.",
                Context = "Payment",
                Action = "PaymentReminder",
                Data = System.Text.Json.JsonSerializer.Serialize(new
                {
                    originalNotificationId = notification.NotificationId,
                    reminderType = "PaymentTimeout"
                }),
                ExpiresAt = DateTime.UtcNow.AddHours(2) // Expire in 2 hours
            },
            "Bill" when notification.Action == "BillOverdue" => new CreateNotificationCommand
            {
                ExternalUserId = notification.ExternalUserId,
                Title = "یادآوری فوری فاکتور",
                Message = "فاکتور شما منقضی شده است. لطفاً هرچه سریع‌تر پرداخت کنید تا از جریمه اضافی جلوگیری شود.",
                Context = "Bill",
                Action = "BillUrgentReminder",
                Data = System.Text.Json.JsonSerializer.Serialize(new
                {
                    originalNotificationId = notification.NotificationId,
                    reminderType = "OverdueUrgent"
                }),
                ExpiresAt = DateTime.UtcNow.AddDays(1) // Expire in 1 day
            },
            "TourReservation" when notification.Action == "ReservationCreated" => new CreateNotificationCommand
            {
                ExternalUserId = notification.ExternalUserId,
                Title = "تایید رزرو تور",
                Message = "لطفاً اطلاعات رزرو خود را بررسی و تایید کنید. در صورت عدم تایید، رزرو شما لغو خواهد شد.",
                Context = "TourReservation",
                Action = "ReservationConfirmationReminder",
                Data = System.Text.Json.JsonSerializer.Serialize(new
                {
                    originalNotificationId = notification.NotificationId,
                    reminderType = "ConfirmationRequired"
                }),
                ExpiresAt = DateTime.UtcNow.AddHours(24) // Expire in 24 hours
            },
            _ => throw new InvalidOperationException($"No follow-up notification defined for context {notification.Context} and action {notification.Action}")
        };
    }
}

/// <summary>
/// Event raised when a notification is successfully saved
/// </summary>
public class NotificationSavedEvent : INotification
{
    public Guid NotificationId { get; set; }
    public Guid ExternalUserId { get; set; }
    public string Context { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public DateTime SavedAt { get; set; }
}
