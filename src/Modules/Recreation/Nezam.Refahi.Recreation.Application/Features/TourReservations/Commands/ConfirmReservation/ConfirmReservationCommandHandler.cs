using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Recreation.Contracts.IntegrationEvents;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;
using Nezam.Refahi.Recreation.Application.Services;
using Nezam.Refahi.Shared.Application;
using MassTransit;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.ConfirmReservation;

/// <summary>
/// Handler for confirming tour reservations after payment
/// </summary>
public class ConfirmReservationCommandHandler : IRequestHandler<ConfirmReservationCommand, ApplicationResult>
{
    private readonly ITourReservationRepository _reservationRepository;
    private readonly IRecreationUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly MemberValidationService _memberValidationService;
    private readonly ILogger<ConfirmReservationCommandHandler> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public ConfirmReservationCommandHandler(
        ITourReservationRepository reservationRepository,
        IRecreationUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        MemberValidationService memberValidationService,
        ILogger<ConfirmReservationCommandHandler> logger,
        IPublishEndpoint publishEndpoint)
    {
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _memberValidationService = memberValidationService;
        _logger = logger;
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
    }

    public async Task<ApplicationResult> Handle(ConfirmReservationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            

            _logger.LogInformation("Confirming reservation - ReservationId: {ReservationId}, TotalAmount: {TotalAmount}, PaymentReference: {PaymentReference}",
                request.ReservationId, request.TotalAmountRials, request.PaymentReference);

            // Get the reservation
            var reservation = await _reservationRepository.FindOneAsync(x => x.Id == request.ReservationId, cancellationToken);

            if (reservation == null)
            {
                _logger.LogWarning("Reservation not found - ReservationId: {ReservationId}", request.ReservationId);
                return ApplicationResult.Failure("رزرو مورد نظر یافت نشد");
            }

           
            // Prepare total amount if provided
            Money? totalAmount = null;
            if (request.TotalAmountRials.HasValue)
            {
                totalAmount = new Money(request.TotalAmountRials.Value);
            }

            // Confirm the reservation
            try
            {
                // For payment-confirmed reservations, we should allow confirmation even if expired
                // because the payment was initiated before expiry
                if (reservation.Status == Nezam.Refahi.Recreation.Domain.Enums.ReservationStatus.PendingConfirmation)
                {
                    // Special handling for payment-confirmed reservations
                    // Skip expiry check since payment was initiated before expiry
                    reservation.Confirm(totalAmount, skipExpiryCheck: true);
                    
                    _logger.LogInformation("Confirmed expired reservation {ReservationId} due to successful payment in PendingConfirmation state",  
                        reservation.Id);
                }
                else
                {
                    // Normal confirmation with expiry check
                    reservation.Confirm(totalAmount);
                }

                // Add payment reference to notes if provided
                if (!string.IsNullOrWhiteSpace(request.PaymentReference))
                {
                    var paymentNote = $"Payment Reference: {request.PaymentReference}";
                    var existingNotes = reservation.Notes;
                    var updatedNotes = string.IsNullOrWhiteSpace(existingNotes)
                        ? paymentNote
                        : $"{existingNotes} | {paymentNote}";
                    reservation.UpdateNotes(updatedNotes);
                }
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Cannot confirm reservation - ReservationId: {ReservationId}, Error: {Error}",
                    request.ReservationId, ex.Message);
                return ApplicationResult.Failure("امکان تایید این رزرو وجود ندارد");
            }

            // Save changes
            await _reservationRepository.UpdateAsync(reservation, cancellationToken: cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully confirmed reservation - ReservationId: {ReservationId}",
                request.ReservationId);

            // Publish ReservationPaymentCompletedIntegrationEvent
            var paymentCompletedEvent = new ReservationPaymentCompletedIntegrationEvent
            {
                ReservationId = reservation.Id,
                BillId = reservation.BillId ?? Guid.Empty,
                BillNumber = reservation.BillId?.ToString() ?? string.Empty,
                AmountRials = (totalAmount?.AmountRials ?? 0),
                PaidAt = DateTime.UtcNow,
                ExternalUserId = reservation.ExternalUserId
            };
            await _publishEndpoint.Publish(paymentCompletedEvent, cancellationToken);

            return ApplicationResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while confirming reservation - ReservationId: {ReservationId}",
                request.ReservationId);
            return ApplicationResult.Failure("خطا در تایید رزرو رخ داده است");
        }
    }

    /// <summary>
    /// Gets the national number for the user - either from request parameter or current authenticated user
    /// </summary>
    /// <param name="requestNationalNumber">National number provided in the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The national number to use for authorization, or null if not found</returns>
    
}