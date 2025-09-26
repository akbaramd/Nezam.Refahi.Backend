using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nezam.Refahi.Notifications.Domain.Repositories;
using Nezam.Refahi.Recreation.Application.Configuration;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Recreation.Domain.ValueObjects;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Application.Services.Contracts;

namespace Nezam.Refahi.WebApi.Services;

/// <summary>
/// Hangfire jobs for cleanup operations that should run at night
/// </summary>
public static class CleanupJobs
{
    /// <summary>
    /// Cleans up expired reservations - runs at 3:00 AM daily
    /// </summary>
    [AutomaticRetry(Attempts = 3)]
    public static async Task CleanupExpiredReservationsAsync(
        IServiceProvider serviceProvider, 
        ILogger logger,
        ReservationSettings settings)
    {
        logger.LogInformation("Starting expired reservations cleanup job");
        
        try
        {
            using var scope = serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<Nezam.Refahi.Recreation.Application.Services.IRecreationUnitOfWork>();
            var reservationRepository = scope.ServiceProvider.GetRequiredService<ITourReservationRepository>();
            var capacityRepository = scope.ServiceProvider.GetRequiredService<ITourCapacityRepository>();

            // Find all expired reservations with single query to avoid race conditions
            var cutoffTime = DateTime.UtcNow.AddMinutes(-settings.GracePeriodMinutes);
            var allExpiredReservations = await reservationRepository.GetExpiredReservationsAsync(cutoffTime, CancellationToken.None);
            
            // Separate reservations by status for different processing
            var heldReservations = allExpiredReservations.Where(r => r.Status == ReservationStatus.Held).ToList();
            var payingReservations = allExpiredReservations.Where(r => r.Status == ReservationStatus.Paying).ToList();

            if (!allExpiredReservations.Any())
            {
                logger.LogInformation("No expired reservations found");
                return;
            }

            logger.LogInformation("Found {Count} expired reservations to process", allExpiredReservations.Count());

            var processedCount = 0;
            var errorCount = 0;

            // Process Held reservations first (immediate cleanup)
            foreach (var reservation in heldReservations)
            {
                try
                {
                    await ProcessHeldExpiredReservation(reservation, capacityRepository, logger);
                    processedCount++;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing expired reservation {ReservationId}", reservation.Id);
                    errorCount++;
                }
            }

            // Process Paying reservations with extended grace period
            foreach (var reservation in payingReservations)
            {
                try
                {
                    await ProcessPayingExpiredReservation(reservation, capacityRepository, settings, logger);
                    processedCount++;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing expired paying reservation {ReservationId}", reservation.Id);
                    errorCount++;
                }
            }

            if (processedCount > 0 || errorCount > 0)
            {
                await unitOfWork.SaveChangesAsync(CancellationToken.None);
                logger.LogInformation("Processed {ProcessedCount} expired reservations, {ErrorCount} errors", 
                    processedCount, errorCount);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during expired reservations cleanup");
            throw; // Let Hangfire handle retry
        }
    }

    /// <summary>
    /// Cleans up old API idempotency records - runs at 3:30 AM daily
    /// </summary>
    [AutomaticRetry(Attempts = 3)]
    public static async Task CleanupOldApiIdempotencyRecordsAsync(
        IServiceProvider serviceProvider, 
        ILogger logger,
        ReservationSettings settings)
    {
        logger.LogInformation("Starting old API idempotency records cleanup job");
        
        try
        {
            using var scope = serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<Nezam.Refahi.Recreation.Application.Services.IRecreationUnitOfWork>();
            var idempotencyRepository = scope.ServiceProvider.GetRequiredService<IApiIdempotencyRepository>();

            var cutoffDate = DateTime.UtcNow.AddMinutes(-settings.IdempotencyTtlMinutes);
            var deletedCount = await idempotencyRepository.DeleteExpiredRecordsAsync(cutoffDate, CancellationToken.None);

            if (deletedCount > 0)
            {
                await unitOfWork.SaveChangesAsync(CancellationToken.None);
                logger.LogInformation("Deleted {Count} old API idempotency records", deletedCount);
            }
            else
            {
                logger.LogInformation("No old API idempotency records to delete");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during API idempotency records cleanup");
            throw; // Let Hangfire handle retry
        }
    }

    /// <summary>
    /// Cleans up expired notifications - runs at 4:00 AM daily
    /// </summary>
    [AutomaticRetry(Attempts = 3)]
    public static async Task CleanupExpiredNotificationsAsync(
        IServiceProvider serviceProvider, 
        ILogger logger)
    {
        logger.LogInformation("Starting expired notifications cleanup job");
        
        try
        {
            using var scope = serviceProvider.CreateScope();
            var notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<Nezam.Refahi.Notifications.Application.Services.INotificationUnitOfWork>();

            // Delete expired notifications
            await notificationRepository.DeleteExpiredAsync(CancellationToken.None);
            var deletedCount = 0; // DeleteExpiredAsync doesn't return count

            if (deletedCount > 0)
            {
                await unitOfWork.SaveChangesAsync(CancellationToken.None);
                logger.LogInformation("Deleted {Count} expired notifications", deletedCount);
            }
            else
            {
                logger.LogInformation("No expired notifications to delete");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during expired notifications cleanup");
            throw; // Let Hangfire handle retry
        }
    }

    /// <summary>
    /// Cleans up expired tokens - runs at 4:30 AM daily
    /// </summary>
    [AutomaticRetry(Attempts = 3)]
    public static async Task CleanupExpiredTokensAsync(
        IServiceProvider serviceProvider, 
        ILogger logger)
    {
        logger.LogInformation("Starting expired tokens cleanup job");
        
        try
        {
            using var scope = serviceProvider.CreateScope();
            var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();

            // Clean up expired tokens
            var totalCleaned = await tokenService.CleanupExpiredTokensAsync();
            
            logger.LogInformation("Token cleanup completed. Total tokens cleaned: {TotalCount}", totalCleaned);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during expired tokens cleanup");
            throw; // Let Hangfire handle retry
        }
    }

    #region Helper Methods

    private static async Task ProcessHeldExpiredReservation(
        TourReservation reservation, 
        ITourCapacityRepository capacityRepository,
        ILogger logger)
    {
        logger.LogDebug("Processing expired reservation {ReservationId} in Held status", reservation.Id);

        // Mark reservation as expired using state machine
        if (ReservationStateMachine.IsValidTransition(reservation.Status, ReservationStatus.Expired))
        {
            reservation.MarkAsExpired();

            // Release capacity for Held reservations
            if (reservation.CapacityId.HasValue)
            {
                var capacity = await capacityRepository.GetByIdAsync(reservation.CapacityId.Value, CancellationToken.None);
                if (capacity != null)
                {
                    var participantCount = reservation.GetParticipantCount();
                    capacity.ReleaseParticipants(participantCount);
                    
                    logger.LogInformation("Released {Count} participants from capacity {CapacityId} (expired Held reservation)", 
                        participantCount, capacity.Id);
                }
            }

            logger.LogInformation("Reservation {ReservationId} marked as expired", reservation.Id);
        }
        else
        {
            logger.LogWarning("Cannot expire reservation {ReservationId} in status {Status}", 
                reservation.Id, reservation.Status);
        }
    }

    private static async Task ProcessPayingExpiredReservation(
        TourReservation reservation, 
        ITourCapacityRepository capacityRepository,
        ReservationSettings settings,
        ILogger logger)
    {
        logger.LogDebug("Processing expired reservation {ReservationId} in Paying status", reservation.Id);

        // Check if this Paying reservation has been expired for too long
        var totalGracePeriodMinutes = settings.GracePeriodMinutes + settings.PaymentCallbackGracePeriodMinutes;
        var expiryThreshold = DateTime.UtcNow.AddMinutes(-totalGracePeriodMinutes);
        
        if (reservation.ExpiryDate.HasValue && reservation.ExpiryDate.Value <= expiryThreshold)
        {
            // Mark as payment failed and release capacity
            reservation.MarkPaymentFailed($"Payment expired - no callback received within {totalGracePeriodMinutes} minutes");
            
            if (reservation.CapacityId.HasValue)
            {
                var capacity = await capacityRepository.GetByIdAsync(reservation.CapacityId.Value, CancellationToken.None);
                if (capacity != null)
                {
                    var participantCount = reservation.GetParticipantCount();
                    capacity.ReleaseParticipants(participantCount);
                    
                    logger.LogInformation("Released {Count} participants from capacity {CapacityId} (expired Paying reservation - {TotalGracePeriod}+ minutes)", 
                        participantCount, capacity.Id, totalGracePeriodMinutes);
                }
            }
        }
        else
        {
            logger.LogWarning("Expired reservation {ReservationId} in Paying status - capacity not yet released (within {TotalGracePeriod} minutes)", 
                reservation.Id, totalGracePeriodMinutes);
        }
    }

    #endregion
}
