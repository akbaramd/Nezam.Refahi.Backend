using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Notifications.Application.Features.Notifications.Commands.CreateNotification;
using Nezam.Refahi.Notifications.Application.Services;
using Nezam.Refahi.Recreation.Domain.Events;

namespace Nezam.Refahi.Notifications.Application.EventHandlers.Recreation;

/// <summary>
/// Event handler for tour created events from Recreation context
/// </summary>
public class TourCreatedEventHandler : INotificationHandler<TourCreatedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<TourCreatedEventHandler> _logger;
    
    public TourCreatedEventHandler(
        INotificationService notificationService,
        ILogger<TourCreatedEventHandler> logger)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task Handle(TourCreatedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling tour created event: {TourTitle}, price {Price}", 
                notification.Title, notification.Price);
            
            // Create notification for all users (broadcast)
            // In a real system, you might want to filter users based on preferences
            var command = new CreateNotificationCommand
            {
                ExternalUserId = notification.CreatedBy, // Or broadcast to all users
                Title = "تور جدید اضافه شد",
                Message = $"تور {notification.Title} با قیمت {notification.Price:N0} تومان اضافه شد.",
                Context = "Tour",
                Action = "TourCreated",
                Data = System.Text.Json.JsonSerializer.Serialize(new
                {
                    tourId = notification.TourId,
                    title = notification.Title,
                    price = notification.Price,
                    originalPrice = notification.OriginalPrice,
                    discount = notification.Discount,
                    startDate = notification.StartDate,
                    endDate = notification.EndDate,
                    capacity = notification.Capacity,
                    availableSpots = notification.AvailableSpots,
                    currency = "IRR",
                    highlights = notification.Highlights,
                    images = notification.Images
                }),
                ExpiresAt = notification.StartDate.AddDays(-1) // Expire one day before tour starts
            };
            
            // Create notification
            var result = await _notificationService.CreateNotificationAsync(command);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("Tour created notification sent successfully for tour {TourId}", notification.TourId);
            }
            else
            {
                _logger.LogError("Failed to send tour created notification for tour {TourId}: {Errors}", 
                    notification.TourId, string.Join(", ", result.Errors));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling tour created event for tour {TourId}", notification.TourId);
        }
    }
}

