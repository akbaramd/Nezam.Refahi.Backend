using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Recreation.Application.Services;
using Nezam.Refahi.Recreation.Contracts.IntegrationEvents;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Application;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;
using MassTransit;

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
    private readonly MemberValidationService _memberValidationService;
    private readonly ILogger<CancelReservationCommandHandler> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public CancelReservationCommandHandler(
        ITourReservationRepository reservationRepository,
        ITourRepository tourRepository,
        IRecreationUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        MemberValidationService memberValidationService,
        ILogger<CancelReservationCommandHandler> logger,
        IPublishEndpoint publishEndpoint)
    {
        _reservationRepository = reservationRepository;
        _tourRepository = tourRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _memberValidationService = memberValidationService;
        _logger = logger;
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
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

            // Get tour for business rule validation
            var tour = await _tourRepository.FindOneAsync(x => x.Id == reservation.TourId, cancellationToken);
            if (tour == null)
            {
                return ApplicationResult<CancelReservationResponse>.Failure("تور مورد نظر یافت نشد");
            }

            // Check authorization if member ID is available
            if (memberId.HasValue)
            {
                var hasAccess = reservation.MemberId == memberId.Value;

                if (!hasAccess )
                {
                    _logger.LogWarning("User does not have access to reservation - ReservationId: {ReservationId}, MemberId: {MemberId}",
                        request.ReservationId, memberId);
                    return ApplicationResult<CancelReservationResponse>.Failure("شما به این رزرو دسترسی ندارید");
                }
            }
            else
            {
                // If no member ID is available and user is not authenticated, deny access
                if (!_currentUserService.IsAuthenticated )
                {
                    _logger.LogWarning("Unauthorized access attempt to cancel reservation - ReservationId: {ReservationId}",
                        request.ReservationId);
                    return ApplicationResult<CancelReservationResponse>.Failure("شما به این رزرو دسترسی ندارید");
                }
            }

            // Apply business rule validation
            var businessRuleValidation = ValidateCancellationBusinessRules(reservation, tour);
            if (!businessRuleValidation.IsValid)
            {
                _logger.LogWarning("Business rule validation failed - ReservationId: {ReservationId}, Error: {Error}",
                    request.ReservationId, businessRuleValidation.ErrorMessage);
                return ApplicationResult<CancelReservationResponse>.Failure(businessRuleValidation.ErrorMessage);
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
                await SafeDeleteReservationAsync(reservation, tour, request.Reason, cancellationToken);
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

            // Publish ReservationCancelledIntegrationEvent
            var reservationCancelledEvent = new ReservationCancelledIntegrationEvent
            {
                ReservationId = request.ReservationId,
                TrackingCode = trackingCode,
                TourId = reservation.TourId,
                TourTitle = tour.Title,
                ReservationDate = reservation.ReservationDate,
                CancelledAt = cancellationDate,
                ExternalUserId = memberId ?? Guid.Empty,
                UserFullName = string.Empty, // Would need to get from member service
                UserNationalCode = string.Empty, // Would need to get from member service
                CancellationReason = request.Reason ?? string.Empty,
                Status = reservation.Status.ToString(),
                WasDeleted = request.PermanentDelete,
                RefundableAmountRials = refundableAmount?.AmountRials,
                PaidAmountRials = reservation.PaidAmount?.AmountRials,
                Currency = "IRR",
                ParticipantCount = participantCount,
                Metadata = new Dictionary<string, string>
                {
                    ["TourId"] = tour.Id.ToString(),
                    ["TourTitle"] = tour.Title,
                    ["CancelledAt"] = cancellationDate.ToString("O"),
                    ["WasDeleted"] = request.PermanentDelete.ToString(),
                    ["CancellationReason"] = request.Reason ?? string.Empty
                }
            };
            await _publishEndpoint.Publish(reservationCancelledEvent, cancellationToken);

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
    /// <param name="tour">The associated tour</param>
    /// <param name="reason">Cancellation reason</param>
    /// <param name="cancellationToken">Cancellation token</param>
    private async Task SafeDeleteReservationAsync(TourReservation reservation, Tour tour, string? reason, CancellationToken cancellationToken)
    {
        try
        {
            // Start a transaction to ensure all-or-nothing deletion
            await _unitOfWork.BeginAsync(cancellationToken);

            try
            {
                // 1. Release tour capacity if the reservation was using capacity (idempotent)
                await ReleaseCapacityIfNeededAsync(reservation, tour, cancellationToken);

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
                var member = await _memberValidationService.GetMemberInfoAsync(_currentUserService.UserNationalNumber ?? string.Empty);
                return member?.Id;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to retrieve member information for user {UserId}", _currentUserService.UserId);
            }
        }

        return null;
    }

    /// <summary>
    /// Validates business rules for cancellation
    /// </summary>
    /// <param name="reservation">The reservation to cancel</param>
    /// <param name="tour">The associated tour</param>
    /// <param name="isOperatorRequest">Whether this is an operator request</param>
    /// <returns>Validation result</returns>
    private (bool IsValid, string ErrorMessage) ValidateCancellationBusinessRules(
        TourReservation reservation, 
        Tour tour)
    {
        var now = DateTime.UtcNow;
        
        // Guard 1: Tour has started - only operators can cancel
        if (now >= tour.TourStart )
        {
            return (false, "امکان لغو رزرو بعد از شروع تور وجود ندارد");
        }
        
        // Guard 2: Cancellation deadline (24 hours before tour)
        var cancellationDeadline = tour.TourStart.AddHours(-24);
        if (now >= cancellationDeadline)
        {
            return (false, "مهلت لغو رزرو (24 ساعت قبل از شروع تور) گذشته است");
        }
        
        // Guard 3: Special handling for Paying state - prevent race with PSP callback
        if (reservation.Status == ReservationStatus.PendingConfirmation)
        {
            return (false, "رزرو در حال پردازش پرداخت است. لطفاً چند دقیقه صبر کنید و مجدداً تلاش کنید");
        }
        
        // Guard 4: Already cancelled states
        if (reservation.Status == ReservationStatus.Cancelled || 
            reservation.Status == ReservationStatus.SystemCancelled )
        {
            return (false, "این رزرو قبلاً لغو شده است");
        }
        
        return (true, string.Empty);
    }

    /// <summary>
    /// Releases capacity for a reservation in an idempotent manner
    /// </summary>
    /// <param name="reservation">The reservation</param>
    /// <param name="tour">The tour</param>
    /// <param name="cancellationToken">Cancellation token</param>
    private async Task ReleaseCapacityIfNeededAsync(
        TourReservation reservation, 
        Tour tour, 
        CancellationToken cancellationToken)
    {
        // Only release capacity for reservations that actually held capacity
        // Confirmed reservations held capacity, others might not have
        if (!reservation.CapacityId.HasValue || 
            (reservation.Status != ReservationStatus.Confirmed && 
             reservation.Status != ReservationStatus.OnHold))
        {
            return;
        }

        var capacity = tour.Capacities.FirstOrDefault(c => c.Id == reservation.CapacityId.Value);
        if (capacity != null)
        {
            // Idempotent capacity release - only release if not already released
            var participantCount = reservation.Participants.Count;
            
            _logger.LogInformation("Releasing capacity for reservation {ReservationId}: {ParticipantCount} participants",
                reservation.Id, participantCount);
                
            capacity.ReleaseParticipants(participantCount);
            await _tourRepository.UpdateAsync(tour, cancellationToken: cancellationToken);
            
            // TODO: Trigger waitlist promotion event here
            // await _eventBus.PublishAsync(new CapacityReleasedEvent(tour.Id, capacity.Id, participantCount), cancellationToken);
        }
    }
}