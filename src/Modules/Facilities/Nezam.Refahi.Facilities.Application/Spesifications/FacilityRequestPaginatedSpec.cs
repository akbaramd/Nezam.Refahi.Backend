using MCA.SharedKernel.Domain.Contracts.Specifications;
using Nezam.Refahi.Facilities.Domain.Entities;
using Nezam.Refahi.Facilities.Domain.Enums;
using Nezam.Refahi.Facilities.Domain.Repositories;

namespace Nezam.Refahi.Facilities.Application.Spesifications;

public sealed class FacilityRequestPaginatedSpec : IPaginatedSpecification<FacilityRequest>
{
    public FacilityRequestPaginatedSpec(
        int pageNumber,
        int pageSize,
        Guid? facilityId,
        Guid? facilityCycleId,
        Guid? memberId,
        string? status,
        string? searchTerm,
        DateTime? dateFrom,
        DateTime? dateTo)
    {
        if (pageNumber <= 0) throw new ArgumentOutOfRangeException(nameof(pageNumber));
        if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));

        PageNumber = pageNumber;
        PageSize = pageSize;
        FacilityId = facilityId;
        FacilityCycleId = facilityCycleId;
        MemberId = memberId;
        Status = string.IsNullOrWhiteSpace(status) ? null : status.Trim();
        SearchTerm = string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm.Trim();
        DateFrom = dateFrom;
        DateTo = dateTo;
    }

    public int PageNumber { get; }
    public int PageSize { get; }
    public Guid? FacilityId { get; }
    public Guid? FacilityCycleId { get; }
    public Guid? MemberId { get; }
    public string? Status { get; }
    public string? SearchTerm { get; }
    public DateTime? DateFrom { get; }
    public DateTime? DateTo { get; }

    public IQueryable<FacilityRequest> Apply(IQueryable<FacilityRequest> query)
    {
        var q = query;

        if (FacilityId.HasValue)
            q = q.Where(r => r.FacilityId == FacilityId.Value);

        if (FacilityCycleId.HasValue)
            q = q.Where(r => r.FacilityCycleId == FacilityCycleId.Value);

        if (MemberId.HasValue)
            q = q.Where(r => r.MemberId == MemberId.Value);

        if (!string.IsNullOrWhiteSpace(Status) && Enum.TryParse<FacilityRequestStatus>(Status, true, out var statusEnum))
            q = q.Where(r => r.Status == statusEnum);

        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            var term = SearchTerm!.ToLower();
            q = q.Where(r =>
                (r.RequestNumber != null && r.RequestNumber.ToLower().Contains(term)) ||
                (r.UserFullName != null && r.UserFullName.ToLower().Contains(term)) ||
                (r.UserNationalId != null && r.UserNationalId.ToLower().Contains(term))
            );
        }

        if (DateFrom.HasValue)
            q = q.Where(r => r.CreatedAt >= DateFrom.Value);
        if (DateTo.HasValue)
            q = q.Where(r => r.CreatedAt <= DateTo.Value);

        q = q.OrderByDescending(r => r.CreatedAt).ThenByDescending(r => r.Id);
        return q;

    }

   
}


