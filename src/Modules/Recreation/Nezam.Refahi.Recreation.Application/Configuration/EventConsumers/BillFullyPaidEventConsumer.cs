using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Contracts.Finance.v1.Messages;
using Nezam.Refahi.Finance.Contracts.IntegrationEvents;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.ConfirmReservation;
using Nezam.Refahi.Recreation.Application.Services;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;

namespace Nezam.Refahi.Recreation.Application.EventConsumers;

/// <summary>
/// Handles BillFullyPaidIntegrationEvent to confirm tour reservations
/// When a bill is fully paid, the associated tour reservation should be confirmed
/// </summary>
public class BillFullyPaidEventConsumer : INotificationHandler<BillFullyPaidEventMessage>
{
    private readonly ITourReservationRepository _reservationRepository;
    private readonly IRecreationUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly ILogger<BillFullyPaidEventConsumer> _logger;

    public BillFullyPaidEventConsumer(
        ITourReservationRepository reservationRepository,
        IRecreationUnitOfWork unitOfWork,
        IMediator mediator,
        ILogger<BillFullyPaidEventConsumer> logger)
    {
        _reservationRepository = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(BillFullyPaidEventMessage notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing BillFullyPaidEventMessage for Bill {BillId}, ReferenceId: {ReferenceId}", 
notification.BillId, notification.ReferenceId);

            // Check if this is a tour reservation bill
            if (notification.ReferenceType != "TourReservation")
            {
                _logger.LogDebug("Ignoring BillFullyPaidEventMessage - ReferenceType {ReferenceType} is not for tour reservation", 
                    notification.ReferenceType);
                return;
            }

            // Find reservation by tracking code (ReferenceId is now the tracking code)
            var reservation = await _reservationRepository.GetByIdAsync(notification.ReferenceId, cancellationToken);
            
            if (reservation == null)
            {
                _logger.LogWarning("No reservation found with tracking code {TrackingCode} for BillFullyPaidEventMessage", 
                    notification.ReferenceId);
                return;
            }

     


            // Confirm the reservation using the existing command
            var confirmCommand = new ConfirmReservationCommand
            {
                ReservationId = reservation.Id,
                TotalAmountRials = (long?)notification.PaidAmountRials, // System confirmation due to payment
                PaymentReference = notification.ReferenceId.ToString() // System confirmation due to payment
            };

            var result = await _mediator.Send(confirmCommand, cancellationToken);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Successfully confirmed reservation {ReservationId} due to full payment of bill {BillId}", 
                    reservation.Id, notification.BillId);
            }
            else
            {
                _logger.LogError("Failed to confirm reservation {ReservationId} due to full payment of bill {BillId}. Errors: {Errors}", 
                    reservation.Id, notification.BillId, string.Join(", ", result.Errors ?? new List<string>()));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing BillFullyPaidEventMessage for Bill {BillId}, ReferenceId: {ReferenceId}", 
                notification.BillId, notification.ReferenceId);
            throw;
        }
    }
}
