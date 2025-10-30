using MCA.SharedKernel.Domain.Contracts.Specifications;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;

namespace Nezam.Refahi.Web.Application.Specifications;

/// <summary>
/// Specification for paginated tour listing with filtering and sorting
/// </summary>
public sealed class TourPaginatedSpec : IPaginatedSpecification<Tour>
{
    public TourPaginatedSpec(
        int pageNumber, 
        int pageSize, 
        string? searchTerm = null,
        TourStatus? status = null,
        bool? isActive = null,
        DateTime? startDateFrom = null,
        DateTime? startDateTo = null,
        long? minPriceRials = null,
        long? maxPriceRials = null,
        int? minAge = null,
        int? maxAge = null,
        string? sortBy = null,
        bool sortDescending = false)
    {
        if (pageNumber <= 0) throw new ArgumentOutOfRangeException(nameof(pageNumber));
        if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));

        PageNumber = pageNumber;
        PageSize = pageSize;
        SearchTerm = string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm.Trim();
        Status = status;
        IsActive = isActive;
        StartDateFrom = startDateFrom;
        StartDateTo = startDateTo;
        MinPriceRials = minPriceRials;
        MaxPriceRials = maxPriceRials;
        MinAge = minAge;
        MaxAge = maxAge;
        SortBy = string.IsNullOrWhiteSpace(sortBy) ? null : sortBy.Trim();
        SortDescending = sortDescending;
    }

    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? SearchTerm { get; set; }
    public TourStatus? Status { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public long? MinPriceRials { get; set; }
    public long? MaxPriceRials { get; set; }
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }

    public IQueryable<Tour> Apply(IQueryable<Tour> query)
    {
        var q = query;

        // Active filter
        if (IsActive.HasValue)
        {
            q = q.Where(t => t.IsActive == IsActive.Value);
        }

        // Status filter
        if (Status.HasValue)
        {
            q = q.Where(t => t.Status == Status.Value);
        }

        // Date range filter
        if (StartDateFrom.HasValue)
        {
            q = q.Where(t => t.TourStart >= StartDateFrom.Value);
        }

        if (StartDateTo.HasValue)
        {
            q = q.Where(t => t.TourStart <= StartDateTo.Value);
        }

        // Age restrictions filter
        if (MinAge.HasValue)
        {
            q = q.Where(t => !t.MinAge.HasValue || t.MinAge <= MinAge.Value);
        }

        if (MaxAge.HasValue)
        {
            q = q.Where(t => !t.MaxAge.HasValue || t.MaxAge >= MaxAge.Value);
        }

        // Search term across key textual fields
        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            var term = SearchTerm!.Trim().ToLower();
            q = q.Where(t =>
                (t.Title != null && t.Title.ToLower().Contains(term)) ||
                (t.Description != null && t.Description.ToLower().Contains(term))
            );
        }

        // Apply sorting
        q = ApplySorting(q);

        return q;
    }

    private IQueryable<Tour> ApplySorting(IQueryable<Tour> query)
    {
        return SortBy?.ToLower() switch
        {
            "title" => SortDescending 
                ? query.OrderByDescending(t => t.Title)
                : query.OrderBy(t => t.Title),
            "startdate" => SortDescending 
                ? query.OrderByDescending(t => t.TourStart)
                : query.OrderBy(t => t.TourStart),
            "enddate" => SortDescending 
                ? query.OrderByDescending(t => t.TourEnd)
                : query.OrderBy(t => t.TourEnd),
            "status" => SortDescending 
                ? query.OrderByDescending(t => t.Status)
                : query.OrderBy(t => t.Status),
            "maxparticipants" => SortDescending 
                ? query.OrderByDescending(t => t.MaxParticipants)
                : query.OrderBy(t => t.MaxParticipants),
            "createdat" => SortDescending 
                ? query.OrderByDescending(t => t.CreatedAt)
                : query.OrderBy(t => t.CreatedAt),
            _ => query.OrderByDescending(t => t.CreatedAt).ThenByDescending(t => t.Id)
        };
    }
}
