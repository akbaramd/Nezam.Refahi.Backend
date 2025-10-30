using MCA.SharedKernel.Application.Contracts;
using MCA.SharedKernel.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.Tours.Queries.GetToursPaginated;

public sealed class GetToursPaginatedQueryHandler
    : IRequestHandler<GetToursPaginatedQuery, ApplicationResult<PaginatedResult<TourDto>>>
{
    private readonly ITourRepository _tourRepository;
    private readonly IMapper<Tour, TourDto> _tourMapper;
    private readonly ILogger<GetToursPaginatedQueryHandler> _logger;

    public GetToursPaginatedQueryHandler(
        ITourRepository tourRepository,
        IMapper<Tour, TourDto> tourMapper,
        ILogger<GetToursPaginatedQueryHandler> logger)
    {
        _tourRepository = tourRepository ?? throw new ArgumentNullException(nameof(tourRepository));
        _tourMapper = tourMapper ?? throw new ArgumentNullException(nameof(tourMapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult<PaginatedResult<TourDto>>> Handle(
        GetToursPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("GetToursPaginated: page={Page} size={Size} active={Active} search='{Search}'",
                request.PageNumber, request.PageSize, request.IsActive, request.Search);

            // Load (current repo shape returns IEnumerable; ensure materialization once)
            var all = await _tourRepository.FindAsync(_ => true, cancellationToken).ConfigureAwait(false);

            // Filter
            var query = all.AsEnumerable();

            if (request.IsActive.HasValue)
                query = query.Where(t => t.IsActive == request.IsActive.Value);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var term = request.Search.Trim();
                query = query.Where(t =>
                    (!string.IsNullOrEmpty(t.Title) && t.Title.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(t.Description) && t.Description.Contains(term, StringComparison.OrdinalIgnoreCase)));
            }

            // Total
            var totalCount = query.Count();

            // Order + Page
            var tours = query
                .OrderByDescending(t => t.CreatedAt) // deterministic, index-friendly in DB-backed impl
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Map using IMapper
            var items = await Task.WhenAll(tours.Select(t => _tourMapper.MapAsync(t, cancellationToken)));

            var page = new PaginatedResult<TourDto>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            return ApplicationResult<PaginatedResult<TourDto>>.Success(page);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("GetToursPaginated cancelled");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetToursPaginated failed");
            return ApplicationResult<PaginatedResult<TourDto>>.Failure(ex, "خطا در دریافت لیست تورها");
        }
    }
}
