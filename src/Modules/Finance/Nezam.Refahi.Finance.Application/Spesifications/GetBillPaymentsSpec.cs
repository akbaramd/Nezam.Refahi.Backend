using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MCA.SharedKernel.Domain.Contracts.Specifications;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Finance.Domain.Entities;

namespace Nezam.Refahi.Finance.Application.Spesifications;

public class GetBillsByUserSpec : IPaginatedSpecification<Bill>
{
    public Guid ExternalUserId { get; private set; }
    // paginated
    private readonly int _pageNumber;
    private readonly int _pageSize;
    public GetBillsByUserSpec(Guid externalUserId, int pageNumber, int pageSize)
    {
        ExternalUserId = externalUserId;
        _pageNumber = pageNumber;
        _pageSize = pageSize;
    }
  public IQueryable<Bill> Apply(IQueryable<Bill> query)
    {
        // Note: Includes should be handled by the repository's PrepareQuery
        // Apply only filtering and ordering, not pagination
        return query
            .Where(b => b.ExternalUserId == ExternalUserId)
            .OrderByDescending(b => b.IssueDate);
    }
    public int PageNumber => _pageNumber;
    public int PageSize => _pageSize;
}