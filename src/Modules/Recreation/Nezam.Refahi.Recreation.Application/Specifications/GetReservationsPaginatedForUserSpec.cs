using System.Linq;
using MCA.SharedKernel.Domain.Contracts.Specifications;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;

namespace Nezam.Refahi.Recreation.Application.Specifications;

public class GetReservationsPaginatedForUserSpec : IPaginatedSpecification<TourReservation>
{
    public GetReservationsPaginatedForUserSpec(
        Guid externalUserId,
        int pageNumber,
        int pageSize,
        ReservationStatus? status = null,
        string? search = null,
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        ExternalUserId = externalUserId;
        PageNumber = pageNumber;
        PageSize = pageSize;
        Status = status;
        Search = search;
        FromDate = fromDate;
        ToDate = toDate;
    }

    public Guid ExternalUserId { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public ReservationStatus? Status { get; }
    public string? Search { get; }
    public DateTime? FromDate { get; }
    public DateTime? ToDate { get; }

    public IQueryable<TourReservation> Apply(IQueryable<TourReservation> query)
    {
        // Filter by user (required)
        query = query.Where(r => r.ExternalUserId == ExternalUserId);

        // Filter by status if provided
        if (Status.HasValue)
        {
            query = query.Where(r => r.Status == Status.Value);
        }

        // Search by tracking code if provided
        if (!string.IsNullOrWhiteSpace(Search))
        {
            var term = Search.Trim();
            query = query.Where(r =>
                r.TrackingCode.Contains(term) ||
                (r.Tour != null && !string.IsNullOrEmpty(r.Tour.Title) && r.Tour.Title.Contains(term)));
        }

        // Filter by reservation date range if provided
        if (FromDate.HasValue)
        {
            query = query.Where(r => r.ReservationDate >= FromDate.Value);
        }

        if (ToDate.HasValue)
        {
            query = query.Where(r => r.ReservationDate <= ToDate.Value);
        }

        // Order by most recent first
        return query.OrderByDescending(r => r.ReservationDate);
    }
}

