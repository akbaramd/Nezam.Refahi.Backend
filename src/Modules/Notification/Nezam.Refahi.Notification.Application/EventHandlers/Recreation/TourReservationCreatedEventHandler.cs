using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Notifications.Application.Features.Notifications.Commands.CreateNotification;
using Nezam.Refahi.Notifications.Application.Services;
using Nezam.Refahi.Recreation.Domain.Events;

namespace Nezam.Refahi.Notifications.Application.EventHandlers.Recreation;

/// <summary>
/// Event handler for tour reservation created events from Recreation context
/// </summary>
public class TourReservationCreatedEventHandler : INotificationHandler<TourReservationCreatedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<TourReservationCreatedEventHandler> _logger;
    
    public TourReservationCreatedEventHandler(
        INotificationService notificationService,
        ILogger<TourReservationCreatedEventHandler> logger)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task Handle(TourReservationCreatedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling tour reservation created event for user {UserId}, reservation {ReservationId}, tour {TourTitle}", 
                notification.ExternalUserId, notification.ReservationId, notification.TourTitle);
            
            // Create notification command
            var command = new CreateNotificationCommand
            {
                ExternalUserId = notification.ExternalUserId,
                Title = "رزرو تور ایجاد شد",
                Message = $"رزرو شما برای تور {notification.TourTitle} با کد پیگیری {notification.TrackingCode} ایجاد شد.",
                Context = "TourReservation",
                Action = "ReservationCreated",
                Data = System.Text.Json.JsonSerializer.Serialize(new
                {
                    reservationId = notification.ReservationId,
                    tourId = notification.TourId,
                    tourTitle = notification.TourTitle,
                    trackingCode = notification.TrackingCode,
                    reservationDate = notification.ReservationDate,
                    tourStartDate = notification.TourStartDate,
                    tourEndDate = notification.TourEndDate,
                    participantCount = notification.ParticipantCount,
                    totalPrice = notification.TotalPrice,
                    currency = "IRR",
                    status = notification.Status
                }),
                ExpiresAt = notification.TourStartDate.AddDays(-1) // Expire one day before tour starts
            };
            
            // Create notification
            var result = await _notificationService.CreateNotificationAsync(command);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("Tour reservation created notification sent successfully for user {UserId}, reservation {ReservationId}", 
                    notification.ExternalUserId, notification.ReservationId);
            }
            else
            {
                _logger.LogError("Failed to send tour reservation created notification for user {UserId}, reservation {ReservationId}: {Errors}", 
                    notification.ExternalUserId, notification.ReservationId, string.Join(", ", result.Errors));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling tour reservation created event for user {UserId}, reservation {ReservationId}", 
                notification.ExternalUserId, notification.ReservationId);
        }
    }
}

