using MediatR;
using Nezam.Refahi.Finance.Application.DTOs;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Queries.Bills;


/// <summary>Get full bill details by Id. Child collections are toggled by flags.</summary>
public  class GetBillDetailsByIdQuery
  : IRequest<ApplicationResult<BillDetailDto>>
{
  public Guid BillId { get; init; }

}