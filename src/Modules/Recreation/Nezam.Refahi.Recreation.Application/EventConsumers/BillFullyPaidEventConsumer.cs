using MediatR;
using MassTransit;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Contracts.Finance.v1.Messages;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.ConfirmReservation;
using Nezam.Refahi.Recreation.Domain.Repositories;

namespace Nezam.Refahi.Recreation.Application.EventConsumers;

/// <summary>
/// Handles BillFullyPaidEventMessage to confirm tour reservations
/// When a bill is fully paid, the associated tour reservation should be confirmed
/// </summary>
public class BillFullyPaidEventConsumer : IConsumer<BillFullyPaidEventMessage>
{
    private readonly ITourReservationRepository _reservationRepository;
    private readonly IMediator _mediator;
    private readonly ILogger<BillFullyPaidEventConsumer> _logger;

    public BillFullyPaidEventConsumer(
        ITourReservationRepository reservationRepository,
        IMediator mediator,
        ILogger<BillFullyPaidEventConsumer> logger)
    {
        _reservationRepository = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<BillFullyPaidEventMessage> context)
    {
        var notification = context.Message;
        var cancellationToken = context.CancellationToken;
        try
        {
            _logger.LogInformation(
                "Received BillFullyPaidEventMessage for Bill {BillId}, ReferenceType: {ReferenceType}, ReferenceId: {ReferenceId}",
                notification.BillId, notification.ReferenceType, notification.ReferenceId);

            // Check if this is a tour reservation bill (case-insensitive)
            if (!string.Equals(notification.ReferenceType, "TourReservation", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogDebug(
                    "Ignoring BillFullyPaidEventMessage - ReferenceType {ReferenceType} is not for tour reservation",
                    notification.ReferenceType);
                return;
            }

            // Validate ReferenceId (it should be the reservation ID as Guid)
            if (notification.ReferenceId == Guid.Empty)
            {
                _logger.LogWarning(
                    "Invalid ReferenceId (empty Guid) in BillFullyPaidEventMessage for Bill {BillId}",
                    notification.BillId);
                return;
            }

            // Find reservation by ID (ReferenceId is the reservation ID)
            var reservation = await _reservationRepository.GetByIdAsync(notification.ReferenceId, cancellationToken);

            if (reservation == null)
            {
                _logger.LogWarning(
                    "No reservation found with Id {ReservationId} for BillFullyPaidEventMessage (BillId: {BillId})",
                    notification.ReferenceId, notification.BillId);
                return;
            }

            // Validate that the bill belongs to this reservation
            if (reservation.BillId.HasValue && reservation.BillId.Value != notification.BillId)
            {
                _logger.LogWarning(
                    "Bill {BillId} does not match reservation's BillId {ReservationBillId} for reservation {ReservationId}",
                    notification.BillId, reservation.BillId, reservation.Id);
                return;
            }

            // Convert decimal to long safely (round to nearest long)
            long? totalAmountRials = null;
            if (notification.PaidAmountRials > 0)
            {
                try
                {
                    totalAmountRials = Convert.ToInt64(Math.Round(notification.PaidAmountRials, MidpointRounding.AwayFromZero));
                }
                catch (OverflowException)
                {
                    _logger.LogError(
                        "Amount {Amount} is too large to convert to long for reservation {ReservationId}",
                        notification.PaidAmountRials, reservation.Id);
                    totalAmountRials = null;
                }
            }

            // Confirm the reservation using the existing command
            var confirmCommand = new ConfirmReservationCommand
            {
                ReservationId = reservation.Id,
                TotalAmountRials = totalAmountRials,
                PaymentReference = !string.IsNullOrWhiteSpace(notification.ReferenceTrackingCode)
                    ? notification.ReferenceTrackingCode
                    : notification.BillId.ToString()
            };

            var result = await _mediator.Send(confirmCommand, cancellationToken);

            if (result.IsSuccess)
            {
                _logger.LogInformation(
                    "Successfully confirmed reservation {ReservationId} due to full payment of bill {BillId}",
                    reservation.Id, notification.BillId);
            }
            else
            {
                _logger.LogError(
                    "Failed to confirm reservation {ReservationId} due to full payment of bill {BillId}. Errors: {Errors}",
                    reservation.Id, notification.BillId, string.Join(", ", result.Errors ?? new List<string>()));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error processing BillFullyPaidEventMessage for Bill {BillId}, ReferenceId: {ReferenceId}",
                notification.BillId, notification.ReferenceId);
            throw;
        }
    }
}
