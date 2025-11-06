using System.Linq;
using MCA.SharedKernel.Domain.Contracts.Specifications;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;

namespace Nezam.Refahi.Finance.Application.Spesifications;

/// <summary>
/// Specification for paginated user payments with optional filters.
/// Filters by Bill.ExternalUserId and optionally by status, date range, and search term.
/// Includes Bill navigation property.
/// </summary>
public class GetPaymentsPaginatedForUserSpec : IPaginatedSpecification<Payment>
{
    public GetPaymentsPaginatedForUserSpec(
        Guid externalUserId,
        int pageNumber,
        int pageSize,
        PaymentStatus? status = null,
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
    public PaymentStatus? Status { get; }
    public string? Search { get; }
    public DateTime? FromDate { get; }
    public DateTime? ToDate { get; }

    public IQueryable<Payment> Apply(IQueryable<Payment> query)
    {
        // Include Bill (required for ExternalUserId filter)
        query = query.Include(p => p.Bill);

        // Filter by user through Bill
        query = query.Where(p => p.Bill.ExternalUserId == ExternalUserId);

        // Filter by status if provided
        if (Status.HasValue)
        {
            query = query.Where(p => p.Status == Status.Value);
        }

        // Search by gateway reference or bill reference tracking code
        if (!string.IsNullOrWhiteSpace(Search))
        {
            var term = Search.Trim();
            query = query.Where(p =>
                (p.GatewayReference != null && p.GatewayReference.Contains(term)) ||
                (p.GatewayTransactionId != null && p.GatewayTransactionId.Contains(term)) ||
                (p.Bill.ReferenceTrackCode != null && p.Bill.ReferenceTrackCode.Contains(term)) ||
                (p.Bill.BillNumber != null && p.Bill.BillNumber.Contains(term)));
        }

        // Filter by payment creation date range if provided
        if (FromDate.HasValue)
        {
            query = query.Where(p => p.CreatedAt >= FromDate.Value);
        }

        if (ToDate.HasValue)
        {
            query = query.Where(p => p.CreatedAt <= ToDate.Value);
        }

        // Order by most recent first
        return query.OrderByDescending(p => p.CreatedAt);
    }
}

