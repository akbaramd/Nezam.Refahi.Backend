using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Notifications.Application.Features.Notifications.Commands.CreateNotification;
using Nezam.Refahi.Notifications.Application.Services;
using Nezam.Refahi.Recreation.Domain.Events;

namespace Nezam.Refahi.Notifications.Application.EventHandlers.Recreation;

/// <summary>
/// Event handler for tour reservation confirmed events from Recreation context
/// </summary>
public class TourReservationConfirmedEventHandler : INotificationHandler<TourReservationConfirmedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<TourReservationConfirmedEventHandler> _logger;
    
    public TourReservationConfirmedEventHandler(
        INotificationService notificationService,
        ILogger<TourReservationConfirmedEventHandler> logger)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task Handle(TourReservationConfirmedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling tour reservation confirmed event for user {UserId}, reservation {ReservationId}, tour {TourTitle}", 
                notification.ExternalUserId, notification.ReservationId, notification.TourTitle);
            
            // Create notification command
            var command = new CreateNotificationCommand
            {
                ExternalUserId = notification.ExternalUserId,
                Title = "رزرو تور تایید شد",
                Message = $"رزرو شما برای تور {notification.TourTitle} با کد پیگیری {notification.TrackingCode} تایید شد.",
                Context = "TourReservation",
                Action = "ReservationConfirmed",
                Data = System.Text.Json.JsonSerializer.Serialize(new
                {
                    reservationId = notification.ReservationId,
                    tourId = notification.TourId,
                    tourTitle = notification.TourTitle,
                    trackingCode = notification.TrackingCode,
                    tourStartDate = notification.TourStartDate,
                    tourEndDate = notification.TourEndDate,
                    participantCount = notification.ParticipantCount,
                    totalPrice = notification.TotalPrice,
                    currency = "IRR",
                    confirmedAt = notification.ConfirmedAt
                }),
                ExpiresAt = notification.TourStartDate.AddDays(7) // Keep for 7 days after tour starts
            };
            
            // Create notification
            var result = await _notificationService.CreateNotificationAsync(command);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("Tour reservation confirmed notification sent successfully for user {UserId}, reservation {ReservationId}", 
                    notification.ExternalUserId, notification.ReservationId);
            }
            else
            {
                _logger.LogError("Failed to send tour reservation confirmed notification for user {UserId}, reservation {ReservationId}: {Errors}", 
                    notification.ExternalUserId, notification.ReservationId, string.Join(", ", result.Errors));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling tour reservation confirmed event for user {UserId}, reservation {ReservationId}", 
                notification.ExternalUserId, notification.ReservationId);
        }
    }
}

