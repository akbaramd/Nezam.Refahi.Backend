using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MCA.SharedKernel.Domain.Contracts.Specifications;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Application.Specifications;

public class GetToursPaginatedForUserSpec : IPaginatedSpecification<Tour>
{
  public GetToursPaginatedForUserSpec(int pageNumber, int pageSize, bool? isActive = null, string? search = null)
  {
    PageNumber = pageNumber;
    PageSize = pageSize;
    IsActive = isActive;
    Search = search;
  }

  public int PageNumber { get; }
  public int PageSize { get; }
  public bool? IsActive { get; }
  public string? Search { get; }

  public IQueryable<Tour> Apply(IQueryable<Tour> query)
  {
    if (IsActive.HasValue)
      query = query.Where(t => t.IsActive == IsActive.Value);

    if (!string.IsNullOrWhiteSpace(Search))
    {
      var term = Search.Trim();
      query = query.Where(t =>
        (!string.IsNullOrEmpty(t.Title) && t.Title.Contains(term)) ||
        (!string.IsNullOrEmpty(t.Description) && t.Description.Contains(term)));
    }

    return query.OrderByDescending(t => t.CreatedAt);
  }
}