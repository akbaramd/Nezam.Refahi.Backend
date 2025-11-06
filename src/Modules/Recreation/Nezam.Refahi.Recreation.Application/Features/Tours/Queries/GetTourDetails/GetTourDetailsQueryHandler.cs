using MediatR;
using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.Tours.Queries.GetTourDetails;

public class GetTourDetailsQueryHandler : IRequestHandler<GetTourDetailsQuery, ApplicationResult<TourDetailWithUserReservationDto>>
{
    private readonly ITourRepository _tourRepository;
    private readonly IMapper<Tour, TourDetailWithUserReservationDto> _tourDetailWithUserMapper;
    private readonly ITourReservationRepository _reservationRepository;
    private readonly IMapper<TourReservation, ReservationDto> _reservationMapper;
    private readonly ICurrentUserService _currentUser;

    public GetTourDetailsQueryHandler(
        ITourRepository tourRepository,
        IMapper<Tour, TourDetailWithUserReservationDto> tourDetailWithUserMapper,
        ITourReservationRepository reservationRepository,
        IMapper<TourReservation, ReservationDto> reservationMapper,
        ICurrentUserService currentUser)
    {
        _tourRepository = tourRepository ?? throw new ArgumentNullException(nameof(tourRepository));
        _tourDetailWithUserMapper = tourDetailWithUserMapper ?? throw new ArgumentNullException(nameof(tourDetailWithUserMapper));
        _reservationRepository = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
        _reservationMapper = reservationMapper ?? throw new ArgumentNullException(nameof(reservationMapper));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    public async Task<ApplicationResult<TourDetailWithUserReservationDto>> Handle(GetTourDetailsQuery request, CancellationToken cancellationToken)
    {
        if (request.TourId == Guid.Empty)
            return ApplicationResult<TourDetailWithUserReservationDto>.Failure("شناسه تور نامعتبر است");

        // Repository should load related collections needed by the mapper
        var tour = await _tourRepository.GetByIdAsync(request.TourId, cancellationToken);
        if (tour == null)
            return ApplicationResult<TourDetailWithUserReservationDto>.Failure("تور مورد نظر یافت نشد");

        var dto = await _tourDetailWithUserMapper.MapAsync(tour, cancellationToken);

        var userId = request.ExternalUserId ?? (_currentUser.IsAuthenticated ? _currentUser.UserId : null);
        if (userId.HasValue)
        {
            var userReservations = await _reservationRepository.GetByTourIdsAndExternalUserIdAsync(
                new[] { request.TourId }, userId.Value, cancellationToken);
            var chosen = userReservations
                .Where(r => r.Status != ReservationStatus.Cancelled && r.Status != ReservationStatus.SystemCancelled && r.Status != ReservationStatus.Rejected)
                .OrderByDescending(r => r.ReservationDate)
                .FirstOrDefault() ?? userReservations.OrderByDescending(r => r.ReservationDate).FirstOrDefault();

            if (chosen != null)
            {
                dto.Reservation = await _reservationMapper.MapAsync(chosen, cancellationToken);
            }
        }

        return ApplicationResult<TourDetailWithUserReservationDto>.Success(dto);
    }
}


