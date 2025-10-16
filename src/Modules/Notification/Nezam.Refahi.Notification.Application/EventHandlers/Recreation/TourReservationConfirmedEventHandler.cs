using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Notifications.Application.Features.Notifications.Commands.CreateNotification;
using Nezam.Refahi.Notifications.Application.Services;
using Nezam.Refahi.Recreation.Contracts.IntegrationEvents;

namespace Nezam.Refahi.Notifications.Application.EventHandlers.Recreation;

/// <summary>
/// Event handler for tour reservation confirmed events from Recreation context
/// </summary>
public class TourReservationConfirmedEventHandler : INotificationHandler<ReservationPaymentCompletedIntegrationEvent>
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
    
    public async Task Handle(ReservationPaymentCompletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling tour reservation payment completed event for user {UserId}, reservation {ReservationId}, payment {PaymentId}", 
                notification.ExternalUserId, notification.ReservationId, notification.PaymentId);
            
            // Create notification command
            var command = new CreateNotificationCommand
            {
                ExternalUserId = notification.ExternalUserId,
                Title = "پرداخت رزرو تور تکمیل شد",
                Message = $"پرداخت رزرو شما با کد پیگیری {notification.TrackingCode} با موفقیت تکمیل شد.",
                Context = "TourReservation",
                Action = "ReservationPaymentCompleted",
                Data = System.Text.Json.JsonSerializer.Serialize(new
                {
                    reservationId = notification.ReservationId,
                    trackingCode = notification.TrackingCode,
                    paymentId = notification.PaymentId,
                    billId = notification.BillId,
                    billNumber = notification.BillNumber,
                    amountRials = notification.AmountRials,
                    currency = "IRR",
                    paidAt = notification.PaidAt,
                    gatewayTransactionId = notification.GatewayTransactionId,
                    gatewayReference = notification.GatewayReference,
                    gateway = notification.Gateway
                }),
                ExpiresAt = DateTime.UtcNow.AddDays(7) // Keep for 7 days
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

