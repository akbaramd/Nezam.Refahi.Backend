using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Recreation.Domain.ValueObjects;
using Nezam.Refahi.Recreation.Application.Configuration;

namespace Nezam.Refahi.Recreation.Application.Services;

/// <summary>
/// Background service for cleaning up expired reservations and releasing capacity
/// </summary>
public class ReservationCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ReservationCleanupService> _logger;
    private readonly ReservationSettings _settings;

    public ReservationCleanupService(
        IServiceProvider serviceProvider,
        ILogger<ReservationCleanupService> logger,
        IOptions<ReservationSettings> settings)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
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
                
                await Task.Delay(_settings.CleanupIntervalMinutes * 60 * 1000, stoppingToken);
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
                await Task.Delay(_settings.ErrorRetryIntervalMinutes * 60 * 1000, stoppingToken);
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

            // Find all expired reservations with single query to avoid race conditions
            var cutoffTime = DateTime.UtcNow.AddMinutes(-_settings.GracePeriodMinutes);
            var allExpiredReservations = await reservationRepository.GetExpiredReservationsAsync(cutoffTime, cancellationToken);
            
            // Separate reservations by status for different processing
            var heldReservations = allExpiredReservations.Where(r => r.Status == ReservationStatus.OnHold).ToList();
            var payingReservations = allExpiredReservations.Where(r => r.Status == ReservationStatus.PendingConfirmation).ToList();

            if (!allExpiredReservations.Any())
            {
                _logger.LogDebug("هیچ رزرو منقضی شده‌ای یافت نشد");
                return;
            }

            _logger.LogInformation("تعداد {Count} رزرو منقضی شده برای پردازش یافت شد", allExpiredReservations.Count());

            var processedCount = 0;
            var errorCount = 0;

            // Process Held reservations first (immediate cleanup)
            foreach (var reservation in heldReservations)
            {
                try
                {
                    await ProcessHeldExpiredReservation(reservation, capacityRepository, cancellationToken);
                    processedCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "خطا در پردازش رزرو منقضی شده {ReservationId}", reservation.Id);
                    errorCount++;
                }
            }

            // Process Paying reservations with extended grace period
            foreach (var reservation in payingReservations)
            {
                try
                {
                    await ProcessPayingExpiredReservation(reservation, capacityRepository, cancellationToken);
                    processedCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "خطا در پردازش رزرو پرداخت منقضی شده {ReservationId}", reservation.Id);
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

    /// <summary>
    /// پردازش رزروهای منقضی شده در وضعیت Held
    /// </summary>
    private async Task ProcessHeldExpiredReservation(
        TourReservation reservation, 
        ITourCapacityRepository capacityRepository,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("پردازش رزرو منقضی شده {ReservationId} در وضعیت Held", reservation.Id);

        // Mark reservation as expired using state machine
        if (ReservationStateMachine.IsValidTransition(reservation.Status, ReservationStatus.Expired))
        {
            reservation.MarkAsExpired();

            // Release capacity for Held reservations
            if (reservation.CapacityId.HasValue)
            {
                var capacity = await capacityRepository.GetByIdAsync(reservation.CapacityId.Value, cancellationToken);
                if (capacity != null)
                {
                    var participantCount = reservation.GetParticipantCount();
                    capacity.ReleaseParticipants(participantCount);
                    
                    _logger.LogInformation("تعداد {Count} شرکت‌کننده از ظرفیت {CapacityId} آزاد شد (رزرو Held منقضی شده)", 
                        participantCount, capacity.Id);
                }
            }

            _logger.LogInformation("رزرو {ReservationId} به عنوان منقضی شده علامت‌گذاری شد", reservation.Id);
        }
        else
        {
            _logger.LogWarning("امکان انقضای رزرو {ReservationId} در وضعیت {Status} وجود ندارد", 
                reservation.Id, reservation.Status);
        }
    }

    /// <summary>
    /// پردازش رزروهای منقضی شده در وضعیت Paying
    /// </summary>
    private async Task ProcessPayingExpiredReservation(
        TourReservation reservation, 
        ITourCapacityRepository capacityRepository,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("پردازش رزرو منقضی شده {ReservationId} در وضعیت Paying", reservation.Id);

        // Check if this Paying reservation has been expired for too long
        var totalGracePeriodMinutes = _settings.GracePeriodMinutes + _settings.PaymentCallbackGracePeriodMinutes;
        var expiryThreshold = DateTime.UtcNow.AddMinutes(-totalGracePeriodMinutes);
        
        if (reservation.ExpiryDate.HasValue && reservation.ExpiryDate.Value <= expiryThreshold)
        {
            // Mark as payment failed and release capacity
            reservation.MarkPaymentFailed($"پرداخت منقضی شد - هیچ callback در مدت {totalGracePeriodMinutes} دقیقه دریافت نشد");
            
            if (reservation.CapacityId.HasValue)
            {
                var capacity = await capacityRepository.GetByIdAsync(reservation.CapacityId.Value, cancellationToken);
                if (capacity != null)
                {
                    var participantCount = reservation.GetParticipantCount();
                    capacity.ReleaseParticipants(participantCount);
                    
                    _logger.LogInformation("تعداد {Count} شرکت‌کننده از ظرفیت {CapacityId} آزاد شد (رزرو Paying منقضی شده - {TotalGracePeriod}+ دقیقه)", 
                        participantCount, capacity.Id, totalGracePeriodMinutes);
                }
            }
        }
        else
        {
            _logger.LogWarning("رزرو منقضی شده {ReservationId} در وضعیت Paying - ظرفیت هنوز آزاد نشده (در مدت {TotalGracePeriod} دقیقه)", 
                reservation.Id, totalGracePeriodMinutes);
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

            var cutoffDate = DateTime.UtcNow.AddMinutes(-_settings.IdempotencyTtlMinutes);
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
