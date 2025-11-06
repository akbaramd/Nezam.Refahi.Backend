using MCA.SharedKernel.Application.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.ExpireReservation;
using Nezam.Refahi.Recreation.Application.Services;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Queries.GetReservationDetail;

/// <summary>
/// Returns a fully populated ReservationDetailDto using IMapper.
/// Automatically updates reservation status to Expired if expired during read.
/// </summary>
public sealed class GetReservationDetailQueryHandler
    : IRequestHandler<GetReservationDetailQuery, ApplicationResult<ReservationDetailDto>>
{
    private readonly ITourReservationRepository _reservationRepository;
    private readonly ITourRepository _tourRepository;
    private readonly ITourCapacityRepository _capacityRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper<TourReservation, ReservationDetailDto> _reservationDetailMapper;
    private readonly IMediator _mediator;
    private readonly ILogger<GetReservationDetailQueryHandler> _logger;

    public GetReservationDetailQueryHandler(
        ITourReservationRepository reservationRepository,
        ITourRepository tourRepository,
        ITourCapacityRepository capacityRepository,
        ICurrentUserService currentUser,
        IMapper<TourReservation, ReservationDetailDto> reservationDetailMapper,
        IMediator mediator,
        ILogger<GetReservationDetailQueryHandler> logger)
    {
        _reservationRepository = reservationRepository;
        _tourRepository = tourRepository;
        _capacityRepository = capacityRepository;
        _currentUser = currentUser;
        _reservationDetailMapper = reservationDetailMapper;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<ApplicationResult<ReservationDetailDto>> Handle(
        GetReservationDetailQuery request,
        CancellationToken ct)
    {
        try
        {
            // 1) Load reservation (participants + snapshots ideally included by repo to avoid N+1)
            // Prefer a repository method like: GetByIdWithNavigationsAsync(id) if available.
            var reservation = await _reservationRepository.FindOneAsync(r => r.Id == request.ReservationId, ct);
            if (reservation is null)
            {
                _logger.LogWarning("Reservation not found. ReservationId={ReservationId}", request.ReservationId);
                return ApplicationResult<ReservationDetailDto>.Failure("رزرو مورد نظر یافت نشد");
            }

            // 2) Authorization by national number if available
     
            if (!HasAccess(reservation, request.UserNationalNumber, _currentUser.IsAuthenticated))
            {
                _logger.LogWarning("Access denied. ReservationId={ReservationId} NationalNumber={NationalNumber}",
                    request.ReservationId, request.UserNationalNumber);
                return ApplicationResult<ReservationDetailDto>.Failure("شما به این رزرو دسترسی ندارید");
            }

            // 3) Check if reservation is expired and expire it using command if needed
            var previousStatus = reservation.Status;
            
            // Check if reservation should be expired (but not already expired)
            if (previousStatus != ReservationStatus.Expired && reservation.IsExpired())
            {
                _logger.LogInformation("Reservation expired detected during read. Expiring reservation. ReservationId={ReservationId}, PreviousStatus={PreviousStatus}",
                    request.ReservationId, previousStatus);
                
                // Use ExpireReservationCommand to properly expire reservation and cancel associated bills
                var expireCommand = new ExpireReservationCommand(
                    reservationId: reservation.Id,
                    reason: "رزرو منقضی شد");
                
                var expireResult = await _mediator.Send(expireCommand, ct);
                
                if (expireResult.IsSuccess)
                {
                    _logger.LogInformation("Reservation expired successfully. ReservationId={ReservationId}, BillCancelled={BillCancelled}",
                        request.ReservationId, expireResult.Data?.BillCancelled ?? false);
                    
                    // Reload reservation to get updated status
                    reservation = await _reservationRepository.GetByIdAsync(request.ReservationId, ct);
                    if (reservation == null)
                    {
                        _logger.LogWarning("Reservation not found after expiration. ReservationId={ReservationId}", request.ReservationId);
                        return ApplicationResult<ReservationDetailDto>.Failure("رزرو بعد از انقضا یافت نشد");
                    }
                }
                else
                {
                    _logger.LogWarning("Failed to expire reservation. ReservationId={ReservationId}, Error={Error}",
                        request.ReservationId, expireResult.Message ?? "Unknown error");
                    // Continue with current status - don't fail the query
                }
            }

            // 4) Load related aggregates needed for detail view
            //    Tour is always required for brief info
            var tour = await _tourRepository.GetByIdAsync(reservation.TourId, ct);
            if (tour is null)
            {
                _logger.LogError("Tour not found for reservation. TourId={TourId} ReservationId={ReservationId}",
                    reservation.TourId, request.ReservationId);
                return ApplicationResult<ReservationDetailDto>.Failure("اطلاعات تور یافت نشد");
            }

            // 5) Map via IMapper
            var dto = await _reservationDetailMapper.MapAsync(reservation, ct);

            _logger.LogInformation("Reservation detail built. ReservationId={ReservationId}", request.ReservationId);
            return ApplicationResult<ReservationDetailDto>.Success(dto);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operation cancelled. ReservationId={ReservationId}", request.ReservationId);
            throw; // let pipeline handle cancellations correctly
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving reservation detail. ReservationId={ReservationId}", request.ReservationId);
            return ApplicationResult<ReservationDetailDto>.Failure(ex, "خطا در دریافت جزئیات رزرو رخ داده است");
        }
    }

    /// <summary>
    /// Returns true if the caller is allowed to see the reservation.
    /// Logic: if national number is available -> must match a participant; else must be authenticated.
    /// </summary>
    private static bool HasAccess(TourReservation reservation, string? nationalNumber, bool isAuthenticated)
    {
        if (!string.IsNullOrWhiteSpace(nationalNumber))
            return reservation.Participants.Any(p => p.NationalNumber == nationalNumber);

        return isAuthenticated; // fallback: authenticated users without national number allowed? policy says no -> deny
    }

}
