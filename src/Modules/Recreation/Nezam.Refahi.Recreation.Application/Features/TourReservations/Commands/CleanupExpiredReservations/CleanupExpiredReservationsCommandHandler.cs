using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Recreation.Application.Services;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Recreation.Domain.ValueObjects;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.CleanupExpiredReservations;

/// <summary>
/// Handler for cleaning up expired reservations
/// </summary>
public class CleanupExpiredReservationsCommandHandler : IRequestHandler<CleanupExpiredReservationsCommand, ApplicationResult<CleanupExpiredReservationsResponse>>
{
    private readonly ITourReservationRepository _reservationRepository;
    private readonly ITourCapacityRepository _capacityRepository;
    private readonly IApiIdempotencyRepository _idempotencyRepository;
    private readonly IRecreationUnitOfWork _unitOfWork;
    private readonly ILogger<CleanupExpiredReservationsCommandHandler> _logger;

    public CleanupExpiredReservationsCommandHandler(
        ITourReservationRepository reservationRepository,
        ITourCapacityRepository capacityRepository,
        IApiIdempotencyRepository idempotencyRepository,
        IRecreationUnitOfWork unitOfWork,
        ILogger<CleanupExpiredReservationsCommandHandler> logger)
    {
        _reservationRepository = reservationRepository;
        _capacityRepository = capacityRepository;
        _idempotencyRepository = idempotencyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApplicationResult<CleanupExpiredReservationsResponse>> Handle(
        CleanupExpiredReservationsCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting manual cleanup of expired reservations. DryRun: {DryRun}", request.DryRun);

            var response = new CleanupExpiredReservationsResponse
            {
                WasDryRun = request.DryRun
            };

            // Calculate cutoff time
            var cutoffTime = request.CutoffTime ?? DateTime.UtcNow;
            if (request.IncludeGracePeriod)
            {
                cutoffTime = cutoffTime.AddMinutes(-request.GracePeriodMinutes);
            }

            // Process expired reservations
            await ProcessExpiredReservations(cutoffTime, response, request.DryRun, cancellationToken);

            // Cleanup idempotency records if requested
            if (request.CleanupIdempotency)
            {
                await CleanupIdempotencyRecords(response, request.DryRun, cancellationToken);
            }

            // Save changes if not dry run
            if (!request.DryRun && (response.ExpiredReservationsCount > 0 || response.DeletedIdempotencyRecordsCount > 0))
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            _logger.LogInformation("Cleanup completed. Processed: {ExpiredCount}, Released: {ReleasedCount}, Idempotency: {IdempotencyCount}",
                response.ExpiredReservationsCount, response.ReleasedParticipantsCount, response.DeletedIdempotencyRecordsCount);

            return ApplicationResult<CleanupExpiredReservationsResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during manual cleanup of expired reservations");
            return ApplicationResult<CleanupExpiredReservationsResponse>.Failure(
                "An error occurred during cleanup process");
        }
    }

    private async Task ProcessExpiredReservations(
        DateTime cutoffTime,
        CleanupExpiredReservationsResponse response,
        bool dryRun,
        CancellationToken cancellationToken)
    {
        var expiredReservations = await _reservationRepository.GetExpiredReservationsAsync(cutoffTime, cancellationToken);
        
        foreach (var reservation in expiredReservations)
        {
            try
            {
                var previousStatus = reservation.Status.ToString();
                var participantCount = reservation.GetParticipantCount();

                // Track the reservation info
                var reservationInfo = new ProcessedReservationInfo
                {
                    ReservationId = reservation.Id,
                    TrackingCode = reservation.TrackingCode,
                    PreviousStatus = previousStatus,
                    ParticipantCount = participantCount,
                    ExpiryDate = reservation.ExpiryDate,
                    CapacityId = reservation.CapacityId
                };

                if (!dryRun)
                {
                    // Check if transition is valid
                    if (ReservationStateMachine.IsValidTransition(reservation.Status, ReservationStatus.Expired))
                    {
                        reservation.MarkAsExpired();
                        reservationInfo.NewStatus = ReservationStatus.Expired.ToString();

                        // Only release capacity for Held reservations, NOT for Paying reservations
                        // Paying reservations should keep their capacity until payment is processed
                        if (reservation.Status == ReservationStatus.Held && participantCount > 0)
                        {
                            if (reservation.CapacityId.HasValue)
                            {
                                var capacity = await _capacityRepository.GetByIdAsync(reservation.CapacityId.Value, cancellationToken);
                                if (capacity != null)
                                {
                                    capacity.ReleaseParticipants(participantCount);
                                    response.ReleasedParticipantsCount += participantCount;
                                }
                                else
                                {
                                    response.Errors.Add($"ظرفیت {reservation.CapacityId.Value} برای رزرو {reservation.Id} یافت نشد");
                                }
                            }
                        }
                        else if (reservation.Status == ReservationStatus.Paying)
                        {
                            response.Errors.Add($"رزرو منقضی شده {reservation.Id} در وضعیت Paying است - ظرفیت آزاد نشد تا از overbooking جلوگیری شود");
                        }
                    }
                    else
                    {
                        reservationInfo.NewStatus = previousStatus; // No change
                        response.Errors.Add($"امکان انقضای رزرو {reservation.Id} در وضعیت {previousStatus} وجود ندارد");
                        continue;
                    }
                }
                else
                {
                    // Dry run - just simulate
                    if (ReservationStateMachine.IsValidTransition(reservation.Status, ReservationStatus.Expired))
                    {
                        reservationInfo.NewStatus = ReservationStatus.Expired.ToString();
                        if (ReservationStateMachine.IsActiveState(reservation.Status) && participantCount > 0)
                        {
                            response.ReleasedParticipantsCount += participantCount;
                        }
                    }
                    else
                    {
                        reservationInfo.NewStatus = previousStatus;
                        response.Errors.Add($"[DRY RUN] Cannot expire reservation {reservation.Id} in status {previousStatus}");
                        continue;
                    }
                }

                response.ProcessedReservations.Add(reservationInfo);
                response.ExpiredReservationsCount++;
            }
            catch (Exception ex)
            {
                var error = $"Error processing reservation {reservation.Id}: {ex.Message}";
                response.Errors.Add(error);
                _logger.LogError(ex, "Error processing expired reservation {ReservationId}", reservation.Id);
            }
        }
    }

    private async Task CleanupIdempotencyRecords(
        CleanupExpiredReservationsResponse response,
        bool dryRun,
        CancellationToken cancellationToken)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-7); // Keep records for 7 days

            if (!dryRun)
            {
                response.DeletedIdempotencyRecordsCount = await _idempotencyRepository.DeleteExpiredRecordsAsync(cutoffDate, cancellationToken);
            }
            else
            {
                // For dry run, just count what would be deleted
                // This would need to be implemented in the repository
                // For now, simulate the count
                response.DeletedIdempotencyRecordsCount = 0;
            }
        }
        catch (Exception ex)
        {
            var error = $"Error during idempotency cleanup: {ex.Message}";
            response.Errors.Add(error);
            _logger.LogError(ex, "Error during idempotency records cleanup");
        }
    }
}
