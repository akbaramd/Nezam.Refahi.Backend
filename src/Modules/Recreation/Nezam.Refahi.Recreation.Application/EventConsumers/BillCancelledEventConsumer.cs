using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Finance.Domain.Events;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.CancelReservation;
using Nezam.Refahi.Recreation.Application.Services;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;

namespace Nezam.Refahi.Recreation.Application.EventConsumers;

/// <summary>
/// Handles BillCancelledEvent to cancel tour reservations
/// When a bill is cancelled, the associated tour reservation should be cancelled
/// </summary>
public class BillCancelledEventConsumer : INotificationHandler<BillCancelledEvent>
{
    private readonly ITourReservationRepository _reservationRepository;
    private readonly IRecreationUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly ILogger<BillCancelledEventConsumer> _logger;

    public BillCancelledEventConsumer(
        ITourReservationRepository reservationRepository,
        IRecreationUnitOfWork unitOfWork,
        IMediator mediator,
        ILogger<BillCancelledEventConsumer> logger)
    {
        _reservationRepository = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(BillCancelledEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing BillCancelledEvent for Bill {BillId}, ReferenceId: {ReferenceId}", 
notification.BillId, notification.ReferenceId);

            // Check if this is a tour reservation bill
            if (notification.ReferenceType != "TourReservation")
            {
                _logger.LogDebug("Ignoring BillCancelledEvent - ReferenceType {ReferenceType} is not for tour reservation", 
                    notification.ReferenceType);
                return;
            }

            // Find reservation by ReferenceId (which is now the reservation ID)
            if (!Guid.TryParse(notification.ReferenceId, out var reservationId))
            {
                _logger.LogWarning("Invalid reservation ID: {ReferenceId}", notification.ReferenceId);
                return;
            }

            var reservation = await _reservationRepository.GetByIdAsync(reservationId, cancellationToken);
            
            if (reservation == null)
            {
                _logger.LogWarning("No reservation found with ID {ReservationId} for BillCancelledEvent", 
                    reservationId);
                return;
            }

            // Verify the bill ID matches
            if (reservation.BillId != notification.BillId)
            {
                _logger.LogWarning("Bill ID mismatch for reservation {ReservationId}. Expected: {ExpectedBillId}, Actual: {ActualBillId}", 
                    reservation.Id, notification.BillId, reservation.BillId);
                return;
            }

            // Check if reservation is in a cancellable status
            if (reservation.Status == Nezam.Refahi.Recreation.Domain.Enums.ReservationStatus.Cancelled ||
                reservation.Status == Nezam.Refahi.Recreation.Domain.Enums.ReservationStatus.SystemCancelled)
            {
                _logger.LogInformation("Reservation {ReservationId} is already cancelled. Current status: {Status}", 
                    reservation.Id, reservation.Status);
                return;
            }

            // Cancel the reservation using the existing command
            var cancelCommand = new CancelReservationCommand
            {
                ReservationId = reservation.Id,
                Reason = $"Reservation cancelled due to bill cancellation: {notification.Reason}",
                PermanentDelete = false // Keep audit trail
            };

            var result = await _mediator.Send(cancelCommand, cancellationToken);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Successfully cancelled reservation {ReservationId} due to bill cancellation {BillId}", 
                    reservation.Id, notification.BillId);
            }
            else
            {
                _logger.LogError("Failed to cancel reservation {ReservationId} due to bill cancellation {BillId}. Errors: {Errors}", 
                    reservation.Id, notification.BillId, string.Join(", ", result.Errors ?? new List<string>()));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing BillCancelledEvent for Bill {BillId}, ReferenceId: {ReferenceId}", 
                notification.BillId, notification.ReferenceId);
            throw;
        }
    }
}

