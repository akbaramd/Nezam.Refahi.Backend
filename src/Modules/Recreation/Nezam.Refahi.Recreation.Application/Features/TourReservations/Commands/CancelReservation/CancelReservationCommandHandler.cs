using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Recreation.Application.Services;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;
using Nezam.Refahi.Membership.Contracts.Services;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.CancelReservation;

/// <summary>
/// Handler for cancelling tour reservations
/// </summary>
public class CancelReservationCommandHandler : IRequestHandler<CancelReservationCommand, ApplicationResult<CancelReservationResponse>>
{
    private readonly ITourReservationRepository _reservationRepository;
    private readonly ITourRepository _tourRepository;
    private readonly IRecreationUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMemberService _memberService;
    private readonly ILogger<CancelReservationCommandHandler> _logger;

    public CancelReservationCommandHandler(
        ITourReservationRepository reservationRepository,
        ITourRepository tourRepository,
        IRecreationUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMemberService memberService,
        ILogger<CancelReservationCommandHandler> logger)
    {
        _reservationRepository = reservationRepository;
        _tourRepository = tourRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _memberService = memberService;
        _logger = logger;
    }

    public async Task<ApplicationResult<CancelReservationResponse>> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get user member ID from current user
            var memberId = await GetUserMemberIdAsync(cancellationToken);

            _logger.LogInformation("Cancelling reservation - ReservationId: {ReservationId}, Reason: {Reason}, MemberId: {MemberId}, PermanentDelete: {PermanentDelete}",
                request.ReservationId, request.Reason, memberId, request.PermanentDelete);

            // Get the reservation with participants and price snapshots
            var reservation = await _reservationRepository.GetByIdWithParticipantsAsync(request.ReservationId, cancellationToken);

            if (reservation == null)
            {
                _logger.LogWarning("Reservation not found - ReservationId: {ReservationId}", request.ReservationId);
                return ApplicationResult<CancelReservationResponse>.Failure("رزرو مورد نظر یافت نشد");
            }

            // Check authorization if member ID is available
            if (memberId.HasValue)
            {
                var hasAccess = reservation.MemberId == memberId.Value;

                if (!hasAccess)
                {
                    _logger.LogWarning("User does not have access to reservation - ReservationId: {ReservationId}, MemberId: {MemberId}",
                        request.ReservationId, memberId);
                    return ApplicationResult<CancelReservationResponse>.Failure("شما به این رزرو دسترسی ندارید");
                }
            }
            else
            {
                // If no member ID is available and user is not authenticated, deny access
                if (!_currentUserService.IsAuthenticated)
                {
                    _logger.LogWarning("Unauthorized access attempt to cancel reservation - ReservationId: {ReservationId}",
                        request.ReservationId);
                    return ApplicationResult<CancelReservationResponse>.Failure("شما به این رزرو دسترسی ندارید");
                }
            }

            // Store information for response before potential deletion
            var participantCount = reservation.Participants.Count;
            var trackingCode = reservation.TrackingCode;
            var cancellationDate = DateTime.UtcNow;
            var refundableAmount = CalculateRefundableAmount(reservation);

            var response = new CancelReservationResponse
            {
                ReservationId = request.ReservationId,
                TrackingCode = trackingCode,
                ParticipantsRemoved = participantCount,
                CancellationReason = request.Reason,
                CancellationDate = cancellationDate,
                RefundableAmountRials = refundableAmount?.AmountRials
            };

            if (request.PermanentDelete)
            {
                // Perform safe deletion
                await SafeDeleteReservationAsync(reservation, request.Reason, cancellationToken);
                response.WasDeleted = true;

                _logger.LogInformation("Successfully deleted reservation and {ParticipantCount} participants - ReservationId: {ReservationId}",
                    participantCount, request.ReservationId);
            }
            else
            {
                // Just mark as cancelled without deletion
                try
                {
                    reservation.Cancel(request.Reason);
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogWarning("Cannot cancel reservation - ReservationId: {ReservationId}, Error: {Error}",
                        request.ReservationId, ex.Message);
                    return ApplicationResult<CancelReservationResponse>.Failure("امکان لغو این رزرو وجود ندارد");
                }

                // Save changes
                await _reservationRepository.UpdateAsync(reservation, cancellationToken: cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                response.WasDeleted = false;

                _logger.LogInformation("Successfully cancelled reservation - ReservationId: {ReservationId}",
                    request.ReservationId);
            }

            return ApplicationResult<CancelReservationResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while cancelling reservation - ReservationId: {ReservationId}",
                request.ReservationId);
            return ApplicationResult<CancelReservationResponse>.Failure("خطا در لغو رزرو رخ داده است");
        }
    }

    /// <summary>
    /// Safely deletes a reservation and all associated data from the database
    /// </summary>
    /// <param name="reservation">The reservation to delete</param>
    /// <param name="reason">Cancellation reason</param>
    /// <param name="cancellationToken">Cancellation token</param>
    private async Task SafeDeleteReservationAsync(TourReservation reservation, string? reason, CancellationToken cancellationToken)
    {
        try
        {
            // Start a transaction to ensure all-or-nothing deletion
            await _unitOfWork.BeginAsync(cancellationToken);

            try
            {
                // 1. Release tour capacity if the reservation was using capacity
                if (reservation.CapacityId.HasValue && reservation.IsActive())
                {
                    var tour = await _tourRepository.FindOneAsync(x => x.Id == reservation.TourId, cancellationToken);
                    if (tour != null)
                    {
                        var capacity = tour.Capacities.FirstOrDefault(c => c.Id == reservation.CapacityId.Value);
                        if (capacity != null)
                        {
                            // Release the participants count back to capacity
                            capacity.ReleaseParticipants(reservation.Participants.Count);
                            await _tourRepository.UpdateAsync(tour, cancellationToken: cancellationToken);
                        }
                    }
                }

                // 2. Log the cancellation for audit purposes before deletion
                _logger.LogInformation("Deleting reservation {ReservationId} with {ParticipantCount} participants. Reason: {Reason}",
                    reservation.Id, reservation.Participants.Count, reason);

                // 3. Delete the reservation (this will cascade delete participants and price snapshots due to EF configuration)
                await _reservationRepository.DeleteAsync(reservation, cancellationToken: cancellationToken);

                // 4. Save all changes
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // 5. Commit transaction
                await _unitOfWork.CommitAsync(cancellationToken);

                _logger.LogInformation("Successfully completed safe deletion of reservation {ReservationId}", reservation.Id);
            }
            catch (Exception)
            {
                // Rollback transaction on any error
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during safe deletion of reservation {ReservationId}", reservation.Id);
            throw;
        }
    }

    /// <summary>
    /// Calculates the refundable amount for a reservation
    /// </summary>
    /// <param name="reservation">The reservation to calculate refund for</param>
    /// <returns>Refundable amount or null if no refund applicable</returns>
    private Money? CalculateRefundableAmount(TourReservation reservation)
    {
        // If reservation has paid amount and is confirmed, calculate refund based on cancellation policy
        if (reservation.PaidAmount != null && reservation.Status == ReservationStatus.Confirmed)
        {
            // For now, return full paid amount as refundable
            // In a real implementation, you'd apply cancellation policies based on:
            // - How close to the tour date
            // - Tour cancellation policy
            // - Member status, etc.
            return reservation.PaidAmount;
        }

        // No refund for unpaid or non-confirmed reservations
        return null;
    }

    /// <summary>
    /// Gets the member ID for the current authenticated user
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The member ID to use for authorization, or null if not found</returns>
    private async Task<Guid?> GetUserMemberIdAsync(CancellationToken cancellationToken)
    {
        // If user is authenticated, get their member information
        if (_currentUserService.IsAuthenticated && _currentUserService.UserId.HasValue)
        {
            try
            {
                var member = await _memberService.GetMemberByExternalIdAsync(_currentUserService.UserId.Value.ToString());
                return member?.Id;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to retrieve member information for user {UserId}", _currentUserService.UserId);
            }
        }

        return null;
    }
}