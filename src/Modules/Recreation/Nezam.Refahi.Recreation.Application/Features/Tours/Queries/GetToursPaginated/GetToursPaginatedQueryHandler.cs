using MCA.SharedKernel.Application.Contracts;
using MCA.SharedKernel.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Recreation.Application.Specifications;

namespace Nezam.Refahi.Recreation.Application.Features.Tours.Queries.GetToursPaginated;

public sealed class GetToursPaginatedQueryHandler
    : IRequestHandler<GetToursPaginatedQuery, ApplicationResult<PaginatedResult<TourWithUserReservationDto>>>
{
    private readonly ITourRepository _tourRepository;
    private readonly IMapper<Tour, TourWithUserReservationDto> _tourWithUserMapper;
    private readonly IMapper<TourReservation, ReservationSummaryDto> _reservationSummaryMapper;
    private readonly ITourReservationRepository _reservationRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<GetToursPaginatedQueryHandler> _logger;

    public GetToursPaginatedQueryHandler(
        ITourRepository tourRepository,
        IMapper<Tour, TourWithUserReservationDto> tourWithUserMapper,
        IMapper<TourReservation, ReservationSummaryDto> reservationSummaryMapper,
        ITourReservationRepository reservationRepository,
        ICurrentUserService currentUser,
        ILogger<GetToursPaginatedQueryHandler> logger)
    {
        _tourRepository = tourRepository ?? throw new ArgumentNullException(nameof(tourRepository));
        _tourWithUserMapper = tourWithUserMapper ?? throw new ArgumentNullException(nameof(tourWithUserMapper));
        _reservationSummaryMapper = reservationSummaryMapper ?? throw new ArgumentNullException(nameof(reservationSummaryMapper));
        _reservationRepository = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult<PaginatedResult<TourWithUserReservationDto>>> Handle(
        GetToursPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("GetToursPaginated: page={Page} size={Size} active={Active} search='{Search}'",
                request.PageNumber, request.PageSize, request.IsActive, request.Search);

            // Specification-based pagination via repository
            var pageData = await _tourRepository.GetPaginatedAsync(
                new GetToursPaginatedForUserSpec(request.PageNumber, request.PageSize, request.IsActive, request.Search),
                cancellationToken);
            var tours = pageData.Items.ToList();

            // Map base items
            var items = (await Task.WhenAll(tours.Select(t => _tourWithUserMapper.MapAsync(t, cancellationToken)))).ToList();

            // Enrich with user's reservation summaries (explicit ExternalUserId preferred)
            var userId = request.ExternalUserId ?? (_currentUser.IsAuthenticated ? _currentUser.UserId : null);
            if (userId.HasValue)
            {
                var tourIds = tours.Select(t => t.Id).Distinct().ToList();
                var reservations = await _reservationRepository.GetByTourIdsAndExternalUserIdAsync(
                    tourIds, userId.Value, cancellationToken);

                // Select the most recent non-terminal reservation per tour
                var byTour = reservations
                    .GroupBy(r => r.TourId)
                    .ToDictionary(
                        g => g.Key,
                        g => g
                            .Where(r => r.Status != ReservationStatus.Cancelled && r.Status != ReservationStatus.SystemCancelled && r.Status != ReservationStatus.Rejected)
                            .OrderByDescending(r => r.ReservationDate)
                            .FirstOrDefault() ?? g.OrderByDescending(r => r.ReservationDate).First());

                // Apply summaries
                foreach (var dto in items)
                {
                    if (byTour.TryGetValue(dto.Id, out var res))
                    {
                        dto.Reservation = await _reservationSummaryMapper.MapAsync(res, cancellationToken);
                    }
                }
            }

            var page = new PaginatedResult<TourWithUserReservationDto>
            {
                Items = items,
                TotalCount = pageData.TotalCount,
                PageNumber = pageData.PageNumber,
                PageSize = pageData.PageSize
            };

            return ApplicationResult<PaginatedResult<TourWithUserReservationDto>>.Success(page);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("GetToursPaginated cancelled");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetToursPaginated failed");
            return ApplicationResult<PaginatedResult<TourWithUserReservationDto>>.Failure(ex, "خطا در دریافت لیست تورها");
        }
    }
}
