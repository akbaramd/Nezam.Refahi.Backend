using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Notifications.Application.Features.Notifications.Commands.CreateNotification;
using Nezam.Refahi.Notifications.Application.Services;
using Nezam.Refahi.Recreation.Contracts.IntegrationEvents;

namespace Nezam.Refahi.Notifications.Application.EventHandlers.Recreation;

/// <summary>
/// Event handler for tour reservation cancelled events from Recreation context
/// </summary>
public class TourReservationCancelledEventHandler : INotificationHandler<ReservationCancelledIntegrationEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<TourReservationCancelledEventHandler> _logger;
    
    public TourReservationCancelledEventHandler(
        INotificationService notificationService,
        ILogger<TourReservationCancelledEventHandler> logger)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task Handle(ReservationCancelledIntegrationEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling tour reservation cancelled event for user {UserId}, reservation {ReservationId}, tour {TourTitle}", 
                notification.ExternalUserId, notification.ReservationId, notification.TourTitle);
            
            // Create notification command
            var command = new CreateNotificationCommand
            {
                ExternalUserId = notification.ExternalUserId,
                Title = "رزرو تور لغو شد",
                Message = $"رزرو شما برای تور {notification.TourTitle} با کد پیگیری {notification.TrackingCode} لغو شد. {GetCancellationMessage(notification.CancellationReason)}",
                Context = "TourReservation",
                Action = GetActionType(notification.CancellationReason),
                Data = System.Text.Json.JsonSerializer.Serialize(new
                {
                    reservationId = notification.ReservationId,
                    tourId = notification.TourId,
                    tourTitle = notification.TourTitle,
                    trackingCode = notification.TrackingCode,
                    cancellationReason = notification.CancellationReason,
                    cancelledAt = notification.CancelledAt,
                    refundableAmountRials = notification.RefundableAmountRials,
                    paidAmountRials = notification.PaidAmountRials,
                    currency = notification.Currency,
                    wasDeleted = notification.WasDeleted,
                    participantCount = notification.ParticipantCount
                }),
                ExpiresAt = DateTime.UtcNow.AddDays(30) // Keep for 30 days
            };
            
            // Create notification
            var result = await _notificationService.CreateNotificationAsync(command);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("Tour reservation cancelled notification sent successfully for user {UserId}, reservation {ReservationId}", 
                    notification.ExternalUserId, notification.ReservationId);
            }
            else
            {
                _logger.LogError("Failed to send tour reservation cancelled notification for user {UserId}, reservation {ReservationId}: {Errors}", 
                    notification.ExternalUserId, notification.ReservationId, string.Join(", ", result.Errors));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling tour reservation cancelled event for user {UserId}, reservation {ReservationId}", 
                notification.ExternalUserId, notification.ReservationId);
        }
    }
    
    private static string GetCancellationMessage(string cancellationReason)
    {
        return cancellationReason switch
        {
            "UserRequest" => "شما درخواست لغو رزرو را داده‌اید.",
            "PaymentTimeout" => "رزرو به دلیل عدم پرداخت در مهلت مقرر لغو شد.",
            "TourCancelled" => "تور توسط اپراتور لغو شد.",
            "InsufficientParticipants" => "تور به دلیل عدم تکمیل حداقل تعداد شرکت‌کنندگان لغو شد.",
            "WeatherConditions" => "تور به دلیل شرایط نامساعد جوی لغو شد.",
            _ => "رزرو لغو شد."
        };
    }
    
    private static string GetActionType(string cancellationReason)
    {
        return cancellationReason switch
        {
            "UserRequest" => "ReservationCancelledByUser",
            "PaymentTimeout" => "ReservationCancelledByTimeout",
            "TourCancelled" => "ReservationCancelledByOperator",
            "InsufficientParticipants" => "ReservationCancelledInsufficientParticipants",
            "WeatherConditions" => "ReservationCancelledWeather",
            _ => "ReservationCancelled"
        };
    }
}

