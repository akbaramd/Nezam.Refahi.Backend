using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Recreation.Application.Services;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.ReactivateExpiredReservation;

/// <summary>
/// Handler for reactivating expired reservations with capacity checking
/// </summary>
public class ReactivateExpiredReservationCommandHandler : IRequestHandler<ReactivateExpiredReservationCommand, ApplicationResult<ReactivateExpiredReservationResponse>>
{
    private readonly ITourReservationRepository _reservationRepository;
    private readonly ITourRepository _tourRepository;
    private readonly IRecreationUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly MemberValidationService _memberValidationService;
    private readonly ILogger<ReactivateExpiredReservationCommandHandler> _logger;

    public ReactivateExpiredReservationCommandHandler(
        ITourReservationRepository reservationRepository,
        ITourRepository tourRepository,
        IRecreationUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        MemberValidationService memberValidationService,
        ILogger<ReactivateExpiredReservationCommandHandler> logger)
    {
        _reservationRepository = reservationRepository;
        _tourRepository = tourRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _memberValidationService = memberValidationService;
        _logger = logger;
    }

    public async Task<ApplicationResult<ReactivateExpiredReservationResponse>> Handle(ReactivateExpiredReservationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get user member ID from current user
            var memberId = await GetUserMemberIdAsync(cancellationToken);

            _logger.LogInformation("Attempting to reactivate expired reservation - ReservationId: {ReservationId}, MemberId: {MemberId}",
                request.ReservationId, memberId);

            // Get the reservation with participants
            var reservation = await _reservationRepository.GetByIdWithParticipantsAsync(request.ReservationId, cancellationToken);

            if (reservation == null)
            {
                _logger.LogWarning("Reservation not found - ReservationId: {ReservationId}", request.ReservationId);
                return ApplicationResult<ReactivateExpiredReservationResponse>.Failure("رزرو مورد نظر یافت نشد");
            }

            // Check authorization
            if (memberId.HasValue)
            {
                var hasAccess = reservation.MemberId == memberId.Value;

                if (!hasAccess)
                {
                    _logger.LogWarning("User does not have access to reservation - ReservationId: {ReservationId}, MemberId: {MemberId}",
                        request.ReservationId, memberId);
                    return ApplicationResult<ReactivateExpiredReservationResponse>.Failure("شما به این رزرو دسترسی ندارید");
                }
            }
            else if (!_currentUserService.IsAuthenticated)
            {
                _logger.LogWarning("Unauthorized access attempt to reactivate reservation - ReservationId: {ReservationId}",
                    request.ReservationId);
                return ApplicationResult<ReactivateExpiredReservationResponse>.Failure("شما به این رزرو دسترسی ندارید");
            }

            // Check if reservation is expired
            if (reservation.Status != ReservationStatus.Expired)
            {
                _logger.LogWarning("Reservation is not expired - ReservationId: {ReservationId}, Status: {Status}",
                    request.ReservationId, reservation.Status);
                return ApplicationResult<ReactivateExpiredReservationResponse>.Failure("فقط رزروهای منقضی شده قابل فعال‌سازی مجدد هستند");
            }

            // Get the tour and check if it's still valid
            var tour = await _tourRepository.FindOneAsync(x => x.Id == reservation.TourId, cancellationToken);
            if (tour == null)
            {
                _logger.LogWarning("Tour not found for reservation - ReservationId: {ReservationId}, TourId: {TourId}",
                    request.ReservationId, reservation.TourId);
                return ApplicationResult<ReactivateExpiredReservationResponse>.Failure("تور مربوط به این رزرو یافت نشد");
            }

            // Check if tour is still active and in the future
            if (tour.TourStart <= DateTime.UtcNow)
            {
                _logger.LogInformation("Tour has already started, deleting expired reservation - ReservationId: {ReservationId}, TourStart: {TourStart}",
                    request.ReservationId, tour.TourStart);

                await SafeDeleteExpiredReservationAsync(reservation, "تور شروع شده است", cancellationToken);

                return ApplicationResult<ReactivateExpiredReservationResponse>.Success(new ReactivateExpiredReservationResponse
                {
                    ReservationId = request.ReservationId,
                    TrackingCode = reservation.TrackingCode,
                    WasReactivated = false,
                    WasDeleted = true,
                    Reason = "تور شروع شده است",
                    ProcessedDate = DateTime.UtcNow,
                    RequiredCapacity = reservation.Participants.Count
                });
            }

            // Check capacity availability
            var requiredCapacity = reservation.Participants.Count;
            var capacityCheckResult = CheckCapacityAvailability(tour, reservation.CapacityId, requiredCapacity);

            var response = new ReactivateExpiredReservationResponse
            {
                ReservationId = request.ReservationId,
                TrackingCode = reservation.TrackingCode,
                AvailableCapacity = capacityCheckResult.AvailableCapacity,
                RequiredCapacity = requiredCapacity,
                ProcessedDate = DateTime.UtcNow,
                Reason = request.Reason
            };

            if (!capacityCheckResult.HasCapacity)
            {
                _logger.LogInformation("No capacity available, deleting expired reservation - ReservationId: {ReservationId}, Required: {Required}, Available: {Available}",
                    request.ReservationId, requiredCapacity, capacityCheckResult.AvailableCapacity);

                await SafeDeleteExpiredReservationAsync(reservation, "ظرفیت کافی موجود نیست", cancellationToken);

                response.WasReactivated = false;
                response.WasDeleted = true;
                response.Reason = "ظرفیت کافی موجود نیست";

                return ApplicationResult<ReactivateExpiredReservationResponse>.Success(response);
            }

            // Reactivate the reservation
            await ReactivateReservationAsync(reservation, capacityCheckResult.Capacity!, request.Reason, cancellationToken);

            response.WasReactivated = true;
            response.WasDeleted = false;
            response.NewStatus = ReservationStatus.OnHold.ToString();
            response.NewExpiryDate = reservation.ExpiryDate;

            _logger.LogInformation("Successfully reactivated expired reservation - ReservationId: {ReservationId}, NewExpiry: {NewExpiry}",
                request.ReservationId, reservation.ExpiryDate);

            return ApplicationResult<ReactivateExpiredReservationResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while reactivating expired reservation - ReservationId: {ReservationId}",
                request.ReservationId);
            return ApplicationResult<ReactivateExpiredReservationResponse>.Failure("خطا در فعال‌سازی مجدد رزرو رخ داده است");
        }
    }

    /// <summary>
    /// Checks if there's enough capacity available for the reservation
    /// </summary>
    private CapacityCheckResult CheckCapacityAvailability(
        Tour tour, 
        Guid? preferredCapacityId, 
        int requiredCapacity)
    {
        // First try the preferred capacity (if specified)
        if (preferredCapacityId.HasValue)
        {
            var preferredCapacity = tour.Capacities.FirstOrDefault(c => c.Id == preferredCapacityId.Value);
            if (preferredCapacity != null && preferredCapacity.CanAccommodate(requiredCapacity))
            {
                    return new CapacityCheckResult
                {
                    HasCapacity = true,
                    Capacity = preferredCapacity,
                    AvailableCapacity = preferredCapacity.RemainingParticipants
                };
            }
        }

        // Try any available capacity
        foreach (var capacity in tour.Capacities.Where(c => c.CanAccommodate(requiredCapacity)))
        {
            return new CapacityCheckResult
            {
                HasCapacity = true,
                Capacity = capacity,
                AvailableCapacity = capacity.RemainingParticipants
            };
        }

        // No capacity available
        var totalAvailable = tour.Capacities.Sum(c => c.RemainingParticipants);
        return new CapacityCheckResult
        {
            HasCapacity = false,
            Capacity = null,
            AvailableCapacity = totalAvailable
        };
    }

    /// <summary>
    /// Reactivates the reservation with new expiry date and allocates capacity
    /// </summary>
    private async Task ReactivateReservationAsync(
        TourReservation reservation, 
        TourCapacity capacity, 
        string? reason, 
        CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginAsync(cancellationToken);

        try
        {
            // Allocate capacity
            if (!capacity.TryAllocateParticipants(reservation.Participants.Count))
            {
                throw new InvalidOperationException("Failed to allocate capacity during reactivation");
            }

            // Reactivate the reservation (this will need to be added to TourReservation entity)
            reservation.Reactivate(DateTime.UtcNow.AddMinutes(15), reason);

            // Update capacity and reservation
            await _tourRepository.UpdateAsync(capacity.Tour, cancellationToken: cancellationToken);
            await _reservationRepository.UpdateAsync(reservation, cancellationToken: cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// Safely deletes an expired reservation that cannot be reactivated
    /// </summary>
    private async Task SafeDeleteExpiredReservationAsync(TourReservation reservation, string reason, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginAsync(cancellationToken);

        try
        {
            _logger.LogInformation("Deleting expired reservation {ReservationId} with {ParticipantCount} participants. Reason: {Reason}",
                reservation.Id, reservation.Participants.Count, reason);

            // Delete the reservation (this will cascade delete participants and price snapshots)
            await _reservationRepository.DeleteAsync(reservation, cancellationToken: cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Successfully deleted expired reservation {ReservationId}", reservation.Id);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// Gets the member ID for the current authenticated user
    /// </summary>
    private async Task<Guid?> GetUserMemberIdAsync(CancellationToken cancellationToken)
    {
        if (_currentUserService.IsAuthenticated && _currentUserService.UserId.HasValue)
        {
            try
            {
                var member = await _memberValidationService.GetMemberInfoByExternalIdAsync(_currentUserService.UserId.Value.ToString());
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
    /// Result of capacity availability check
    /// </summary>
    private class CapacityCheckResult
    {
        public bool HasCapacity { get; set; }
        public TourCapacity? Capacity { get; set; }
        public int AvailableCapacity { get; set; }
    }
}
