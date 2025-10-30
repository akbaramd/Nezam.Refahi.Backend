using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MCA.SharedKernel.Domain.Contracts.Specifications;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Finance.Domain.Entities;

namespace Nezam.Refahi.Finance.Application.Spesifications;

public class GetBillPaymentsSpec : IPaginatedSpecification<Payment>
{
    public Guid BillId { get; private set; }
    // paginated
    private readonly int _pageNumber;
    private readonly int _pageSize;
    
    public GetBillPaymentsSpec(Guid billId, int pageNumber, int pageSize)
    {
      BillId = billId;
        _pageNumber = pageNumber;
        _pageSize = pageSize;
    }
  public IQueryable<Payment> Apply(IQueryable<Payment> query)
    {
        // Note: Includes should be handled by the repository's PrepareQuery
        // Apply only filtering and ordering, not pagination
        return query
          .Include(x=>x.Bill)
          .Include(x=>x.Transactions)
            .Where(b => b.BillId == BillId)
            .OrderByDescending(b => b.Id);
    }
    public int PageNumber => _pageNumber;
    public int PageSize => _pageSize;
}