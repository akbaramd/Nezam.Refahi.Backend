using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;
using Nezam.Refahi.Membership.Contracts.Services;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.ConfirmReservation;

/// <summary>
/// Handler for confirming tour reservations after payment
/// </summary>
public class ConfirmReservationCommandHandler : IRequestHandler<ConfirmReservationCommand, ApplicationResult>
{
    private readonly ITourReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMemberService _memberService;
    private readonly ILogger<ConfirmReservationCommandHandler> _logger;

    public ConfirmReservationCommandHandler(
        ITourReservationRepository reservationRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMemberService memberService,
        ILogger<ConfirmReservationCommandHandler> logger)
    {
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _memberService = memberService;
        _logger = logger;
    }

    public async Task<ApplicationResult> Handle(ConfirmReservationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get user national number from current user or request parameter
            string? nationalNumber = await GetUserNationalNumberAsync( cancellationToken);

            _logger.LogInformation("Confirming reservation - ReservationId: {ReservationId}, TotalAmount: {TotalAmount}, PaymentReference: {PaymentReference}, NationalNumber: {NationalNumber}",
                request.ReservationId, request.TotalAmountRials, request.PaymentReference, nationalNumber);

            // Get the reservation
            var reservation = await _reservationRepository.FindOneAsync(x => x.Id == request.ReservationId, cancellationToken);

            if (reservation == null)
            {
                _logger.LogWarning("Reservation not found - ReservationId: {ReservationId}", request.ReservationId);
                return ApplicationResult.Failure("رزرو مورد نظر یافت نشد");
            }

            // Check authorization if national number is available
            if (!string.IsNullOrWhiteSpace(nationalNumber))
            {
                var hasAccess = reservation.Participants.Any(p =>
                    p.NationalNumber == nationalNumber);

                if (!hasAccess)
                {
                    _logger.LogWarning("User does not have access to reservation - ReservationId: {ReservationId}, NationalNumber: {NationalNumber}",
                        request.ReservationId, nationalNumber);
                    return ApplicationResult.Failure("شما به این رزرو دسترسی ندارید");
                }
            }
            else
            {
                // If no national number is available and user is not authenticated, deny access
                if (!_currentUserService.IsAuthenticated)
                {
                    _logger.LogWarning("Unauthorized access attempt to confirm reservation - ReservationId: {ReservationId}",
                        request.ReservationId);
                    return ApplicationResult.Failure("شما به این رزرو دسترسی ندارید");
                }
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
                reservation.Confirm(totalAmount);

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
    private async Task<string?> GetUserNationalNumberAsync( CancellationToken cancellationToken)
    {


        // If user is authenticated, get their member information
        if (_currentUserService.IsAuthenticated && _currentUserService.UserId.HasValue)
        {
            try
            {
                var member = await _memberService.GetMemberByExternalIdAsync(_currentUserService.UserId.Value.ToString());
                return member?.NationalCode;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to retrieve member information for user {UserId}", _currentUserService.UserId);
            }
        }

        return null;
    }
}