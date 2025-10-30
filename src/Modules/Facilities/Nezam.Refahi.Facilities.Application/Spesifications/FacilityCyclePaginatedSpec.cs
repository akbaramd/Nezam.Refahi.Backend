using MCA.SharedKernel.Domain.Contracts.Specifications;
using Nezam.Refahi.Facilities.Domain.Entities;
using Nezam.Refahi.Facilities.Domain.Enums;

namespace Nezam.Refahi.Facilities.Application.Spesifications;

public sealed class FacilityCyclePaginatedSpec : IPaginatedSpecification<FacilityCycle>
{
    public FacilityCyclePaginatedSpec(
        Guid facilityId,
        int pageNumber,
        int pageSize,
        string? status,
        string? searchTerm,
        bool onlyActive,
        bool onlyWithUserRequests = false,
        IEnumerable<Guid>? userRequestCycleIds = null)
    {
        if (pageNumber <= 0) throw new ArgumentOutOfRangeException(nameof(pageNumber));
        if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));

        FacilityId = facilityId;
        PageNumber = pageNumber;
        PageSize = pageSize;
        Status = string.IsNullOrWhiteSpace(status) ? null : status.Trim();
        SearchTerm = string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm.Trim();
        OnlyActive = onlyActive;
        OnlyWithUserRequests = onlyWithUserRequests;
        UserRequestCycleIds = userRequestCycleIds != null ? new HashSet<Guid>(userRequestCycleIds) : null;
    }

    public Guid FacilityId { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public string? Status { get; }
    public string? SearchTerm { get; }
    public bool OnlyActive { get; }
    public bool OnlyWithUserRequests { get; }
    public HashSet<Guid>? UserRequestCycleIds { get; }

    public IQueryable<FacilityCycle> Apply(IQueryable<FacilityCycle> query)
    {
        var q = ApplyWithoutPaging(query);

        // paging
        var skip = (PageNumber - 1) * PageSize;
        q = q.Skip(skip).Take(PageSize);

        return q;
    }

    public IQueryable<FacilityCycle> ApplyWithoutPaging(IQueryable<FacilityCycle> query)
    {
        var q = query.Where(c => c.FacilityId == FacilityId);

        if (OnlyActive)
            q = q.Where(c => c.Status == FacilityCycleStatus.Active);

        if (!string.IsNullOrWhiteSpace(Status) &&
            Enum.TryParse<FacilityCycleStatus>(Status, true, out var statusEnum))
            q = q.Where(c => c.Status == statusEnum);

        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            var term = SearchTerm!.ToLower();
            q = q.Where(c =>
                (c.Name != null && c.Name.ToLower().Contains(term)) ||
                (c.Description != null && c.Description.ToLower().Contains(term))
            );
        }

        if (OnlyWithUserRequests && UserRequestCycleIds != null && UserRequestCycleIds.Count > 0)
        {
            q = q.Where(c => UserRequestCycleIds.Contains(c.Id));
        }

        q = q.OrderByDescending(c => c.CreatedAt).ThenByDescending(c => c.Id);

        return q;
    }
}


