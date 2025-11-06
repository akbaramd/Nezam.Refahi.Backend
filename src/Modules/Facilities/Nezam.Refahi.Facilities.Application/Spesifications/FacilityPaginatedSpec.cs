using MCA.SharedKernel.Domain.Contracts.Specifications;
using Nezam.Refahi.Facilities.Domain.Entities;

namespace Nezam.Refahi.Facilities.Application.Spesifications;

/// <summary>
/// Specification داده‌های صفحه‌بندی/فیلتر/جست‌وجو برای فهرست تسهیلات
/// </summary>
 public sealed class FacilityPaginatedSpec : IPaginatedSpecification<Facility>
{
    public FacilityPaginatedSpec(int pageNumber, int pageSize, string? searchTerm, bool onlyActive)
    {
        if (pageNumber <= 0) throw new ArgumentOutOfRangeException(nameof(pageNumber));
        if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));

        PageNumber = pageNumber;
        PageSize = pageSize;
        SearchTerm = string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm.Trim();
        OnlyActive = onlyActive;
    }

    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public string? SearchTerm { get; set; }
    public bool OnlyActive { get; set; }

  public IQueryable<Facility> Apply(IQueryable<Facility> query)
  {
    var q = query;

    // OnlyActive → وضعیت Active
    if (OnlyActive)
    {
      q = q.Where(f => f.Cycles.Any(c => c.Status == Nezam.Refahi.Facilities.Domain.Enums.FacilityCycleStatus.Active));
    }

    // Type filter (case-insensitive parse)
  
    // SearchTerm across key textual fields
    if (!string.IsNullOrWhiteSpace(SearchTerm))
    {
      var term = SearchTerm!.Trim().ToLower();
      q = q.Where(f =>
        (f.Name != null && f.Name.ToLower().Contains(term)) ||
        (f.Code != null && f.Code.ToLower().Contains(term)) ||
        (f.Description != null && f.Description.ToLower().Contains(term)) ||
        (f.BankName != null && f.BankName.ToLower().Contains(term)) ||
        (f.BankCode != null && f.BankCode.ToLower().Contains(term))
      );
    }

    // Default sort: newest first by CreatedAt then Id (stable)
    q = q.OrderByDescending(f => f.CreatedAt).ThenByDescending(f => f.Id);


    return q;
  }


}


