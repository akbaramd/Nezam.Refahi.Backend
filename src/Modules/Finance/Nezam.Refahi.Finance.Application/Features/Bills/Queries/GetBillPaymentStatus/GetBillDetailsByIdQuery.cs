using MediatR;
using Nezam.Refahi.Finance.Application.DTOs;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Queries.Bills;


/// <summary>Get full bill details by Id. Child collections are toggled by flags.</summary>
public  class GetBillDetailsByIdQuery
  : IRequest<ApplicationResult<BillDetailDto>>
{
  public Guid BillId { get; init; }

  /// <summary>
  /// External User ID to ensure user can only access their own bills
  /// </summary>
  public Guid ExternalUserId { get; init; }
}