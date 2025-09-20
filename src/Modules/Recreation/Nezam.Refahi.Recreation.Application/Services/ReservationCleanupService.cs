using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Recreation.Domain.ValueObjects;

namespace Nezam.Refahi.Recreation.Application.Services;

/// <summary>
/// Background service for cleaning up expired reservations and releasing capacity
/// </summary>
public class ReservationCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ReservationCleanupService> _logger;
    private readonly ReservationCleanupOptions _options;

    public ReservationCleanupService(
        IServiceProvider serviceProvider,
        ILogger<ReservationCleanupService> logger,
        IOptions<ReservationCleanupOptions> options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Reservation cleanup service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessExpiredReservations(stoppingToken);
                await CleanupOldApiIdempotencyRecords(stoppingToken);
                
                await Task.Delay(_options.CleanupIntervalMinutes * 60 * 1000, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Reservation cleanup service is stopping");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during reservation cleanup");
                
                // Wait shorter interval on error to retry sooner
                await Task.Delay(_options.ErrorRetryIntervalMinutes * 60 * 1000, stoppingToken);
            }
        }
    }

    private async Task ProcessExpiredReservations(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IRecreationUnitOfWork>();
        var reservationRepository = scope.ServiceProvider.GetRequiredService<ITourReservationRepository>();
        var capacityRepository = scope.ServiceProvider.GetRequiredService<ITourCapacityRepository>();

        try
        {
            _logger.LogDebug("Starting expired reservations cleanup");

            // Find reservations that should be expired
            var expiredReservations = await reservationRepository.GetExpiredReservationsAsync(
                DateTime.UtcNow.AddMinutes(-_options.GracePeriodMinutes), 
                cancellationToken);

            if (!expiredReservations.Any())
            {
                _logger.LogDebug("No expired reservations found");
                return;
            }

            _logger.LogInformation("Found {Count} expired reservations to process", expiredReservations.Count());

            var processedCount = 0;
            var errorCount = 0;

            foreach (var reservation in expiredReservations)
            {
                try
                {
                    await ProcessSingleExpiredReservation(reservation, capacityRepository, cancellationToken);
                    processedCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process expired reservation {ReservationId}", reservation.Id);
                    errorCount++;
                }
            }

            if (processedCount > 0 || errorCount > 0)
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Processed {ProcessedCount} expired reservations, {ErrorCount} errors", 
                    processedCount, errorCount);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during expired reservations cleanup");
            throw;
        }
    }

    private async Task ProcessSingleExpiredReservation(
        TourReservation reservation, 
        ITourCapacityRepository capacityRepository,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Processing expired reservation {ReservationId} with status {Status}", 
            reservation.Id, reservation.Status);

        // Mark reservation as expired using state machine
        if (ReservationStateMachine.IsValidTransition(reservation.Status, ReservationStatus.Expired))
        {
            reservation.MarkAsExpired();

            // Release capacity if reservation was holding it
            if (ReservationStateMachine.IsActiveState(reservation.Status))
            {
                var capacity = await capacityRepository.FindOneAsync(x=>x.Id==reservation.CapacityId, cancellationToken);
                if (capacity != null)
                {
                    var participantCount = reservation.GetParticipantCount();
                    capacity.ReleaseParticipants(participantCount);
                    
                    _logger.LogDebug("Released {Count} participants from capacity {CapacityId}", 
                        participantCount, capacity.Id);
                }
            }

            _logger.LogInformation("Marked reservation {ReservationId} as expired", reservation.Id);
        }
        else
        {
            _logger.LogWarning("Cannot expire reservation {ReservationId} in status {Status}", 
                reservation.Id, reservation.Status);
        }
    }

    private async Task CleanupOldApiIdempotencyRecords(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IRecreationUnitOfWork>();
        var idempotencyRepository = scope.ServiceProvider.GetRequiredService<IApiIdempotencyRepository>();

        try
        {
            _logger.LogDebug("Starting old API idempotency records cleanup");

            var cutoffDate = DateTime.UtcNow.AddDays(-_options.IdempotencyRetentionDays);
            var deletedCount = await idempotencyRepository.DeleteExpiredRecordsAsync(cutoffDate, cancellationToken);

            if (deletedCount > 0)
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Deleted {Count} old API idempotency records", deletedCount);
            }
            else
            {
                _logger.LogDebug("No old API idempotency records to delete");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during API idempotency records cleanup");
            throw;
        }
    }
}

/// <summary>
/// Configuration options for reservation cleanup service
/// </summary>
public class ReservationCleanupOptions
{
    public const string SectionName = "ReservationCleanup";

    /// <summary>
    /// Interval between cleanup runs in minutes (default: 5 minutes)
    /// </summary>
    public int CleanupIntervalMinutes { get; set; } = 5;

    /// <summary>
    /// Grace period before marking reservations as expired in minutes (default: 2 minutes)
    /// </summary>
    public int GracePeriodMinutes { get; set; } = 2;

    /// <summary>
    /// Retry interval on error in minutes (default: 1 minute)
    /// </summary>
    public int ErrorRetryIntervalMinutes { get; set; } = 1;

    /// <summary>
    /// Number of days to retain API idempotency records (default: 7 days)
    /// </summary>
    public int IdempotencyRetentionDays { get; set; } = 7;

    /// <summary>
    /// Maximum number of reservations to process in one batch (default: 100)
    /// </summary>
    public int BatchSize { get; set; } = 100;
}
