using MCA.SharedKernel.Application.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Application.Services;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Queries.GetReservationDetail;

/// <summary>
/// Returns a fully populated ReservationDetailDto using IMapper.
/// </summary>
public sealed class GetReservationDetailQueryHandler
    : IRequestHandler<GetReservationDetailQuery, ApplicationResult<ReservationDetailDto>>
{
    private readonly ITourReservationRepository _reservationRepository;
    private readonly ITourRepository _tourRepository;
    private readonly ITourCapacityRepository _capacityRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly MemberValidationService _memberValidation;
    private readonly IMapper<TourReservation, ReservationDetailDto> _reservationDetailMapper;
    private readonly ILogger<GetReservationDetailQueryHandler> _logger;

    public GetReservationDetailQueryHandler(
        ITourReservationRepository reservationRepository,
        ITourRepository tourRepository,
        ITourCapacityRepository capacityRepository,
        ICurrentUserService currentUser,
        MemberValidationService memberValidation,
        IMapper<TourReservation, ReservationDetailDto> reservationDetailMapper,
        ILogger<GetReservationDetailQueryHandler> logger)
    {
        _reservationRepository = reservationRepository;
        _tourRepository = tourRepository;
        _capacityRepository = capacityRepository;
        _currentUser = currentUser;
        _memberValidation = memberValidation;
        _reservationDetailMapper = reservationDetailMapper;
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

            // 3) Load related aggregates needed for detail view
            //    Tour is always required for brief info
            var tour = await _tourRepository.GetByIdAsync(reservation.TourId, ct);
            if (tour is null)
            {
                _logger.LogError("Tour not found for reservation. TourId={TourId} ReservationId={ReservationId}",
                    reservation.TourId, request.ReservationId);
                return ApplicationResult<ReservationDetailDto>.Failure("اطلاعات تور یافت نشد");
            }

            // 4) Map via IMapper
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
