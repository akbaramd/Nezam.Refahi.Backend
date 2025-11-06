using MCA.SharedKernel.Application.Contracts;
using MCA.SharedKernel.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Recreation.Application.Specifications;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Queries.GetReservationsPaginated;

public sealed class GetReservationsPaginatedQueryHandler
    : IRequestHandler<GetReservationsPaginatedQuery, ApplicationResult<PaginatedResult<ReservationDto>>>
{
    private readonly ITourReservationRepository _reservationRepository;
    private readonly IMapper<TourReservation, ReservationDto> _reservationMapper;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<GetReservationsPaginatedQueryHandler> _logger;

    public GetReservationsPaginatedQueryHandler(
        ITourReservationRepository reservationRepository,
        IMapper<TourReservation, ReservationDto> reservationMapper,
        ICurrentUserService currentUser,
        ILogger<GetReservationsPaginatedQueryHandler> logger)
    {
        _reservationRepository = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
        _reservationMapper = reservationMapper ?? throw new ArgumentNullException(nameof(reservationMapper));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult<PaginatedResult<ReservationDto>>> Handle(
        GetReservationsPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Determine the user ID to use
            var userId = request.ExternalUserId ?? 
                        (_currentUser.IsAuthenticated ? _currentUser.UserId : null);

            if (!userId.HasValue)
            {
                _logger.LogWarning("GetReservationsPaginated: No user ID available");
                return ApplicationResult<PaginatedResult<ReservationDto>>.Failure(
                    "شناسه کاربر الزامی است");
            }

            _logger.LogInformation(
                "GetReservationsPaginated: userId={UserId} page={Page} size={Size} status={Status} search='{Search}'",
                userId.Value, request.PageNumber, request.PageSize, request.Status, request.Search);

            // Specification-based pagination via repository
            var spec = new GetReservationsPaginatedForUserSpec(
                externalUserId: userId.Value,
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                status: request.Status,
                search: request.Search,
                fromDate: request.FromDate,
                toDate: request.ToDate);

            var pageData = await _reservationRepository.GetPaginatedAsync(spec, cancellationToken);
            var reservations = pageData.Items.ToList();

            // Map to DTOs
            var items = (await Task.WhenAll(
                reservations.Select(r => _reservationMapper.MapAsync(r, cancellationToken))))
                .ToList();

            var page = new PaginatedResult<ReservationDto>
            {
                Items = items,
                TotalCount = pageData.TotalCount,
                PageNumber = pageData.PageNumber,
                PageSize = pageData.PageSize
            };

            return ApplicationResult<PaginatedResult<ReservationDto>>.Success(page);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("GetReservationsPaginated cancelled");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetReservationsPaginated failed");
            return ApplicationResult<PaginatedResult<ReservationDto>>.Failure(
                ex, "خطا در دریافت لیست رزروها");
        }
    }
}

