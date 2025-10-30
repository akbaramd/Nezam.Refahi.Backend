using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Finance.Contracts.IntegrationEvents;
using Nezam.Refahi.Recreation.Application.Services;
using Nezam.Refahi.Recreation.Domain.Repositories;

namespace Nezam.Refahi.Recreation.Application.EventConsumers;

/// <summary>
/// Consumer for handling bill creation events from Finance module
/// </summary>
public class BillCreatedForReservationConsumer : INotificationHandler<BillCreatedIntegrationEvent>
{
    private readonly ITourReservationRepository _reservationRepository;
    private readonly IRecreationUnitOfWork _unitOfWork;
    private readonly ILogger<BillCreatedForReservationConsumer> _logger;

    public BillCreatedForReservationConsumer(
        ITourReservationRepository reservationRepository,
        IRecreationUnitOfWork unitOfWork,
        ILogger<BillCreatedForReservationConsumer> logger)
    {
        _reservationRepository = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(BillCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing bill created event for reference {ReferenceType}:{ReferenceId}, bill {BillId}", 
            notification.ReferenceType, notification.ReferenceId, notification.BillId);

        try
        {
            // Only handle TourReservation references
            if (!string.Equals(notification.ReferenceType, "TourReservation", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

        

            // Get the reservation
            var reservation = await _reservationRepository.GetByIdAsync(notification.ReferenceId, cancellationToken);
            if (reservation == null)
            {
                _logger.LogWarning("Reservation {ReservationId} not found for bill {BillId}", 
                  notification.ReferenceId, notification.BillId);
                return;
            }

            // Update reservation with bill information
            reservation.SetBillId(notification.BillId);
            // Note: The reservation status should already be set to Paying by the InitiatePaymentCommandHandler

            await _reservationRepository.UpdateAsync(reservation, cancellationToken:cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully updated reservation {ReservationId} with bill {BillId}", 
              notification.ReferenceId, notification.BillId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing bill created event for reservation ref {ReferenceId}, bill {BillId}", 
                notification.ReferenceId, notification.BillId);
            throw;
        }
    }
}
