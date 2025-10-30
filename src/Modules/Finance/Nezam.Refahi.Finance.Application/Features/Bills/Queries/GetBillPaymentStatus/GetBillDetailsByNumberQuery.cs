using MediatR;
using Nezam.Refahi.Finance.Application.DTOs;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Queries.Bills;

/// <summary>Get full bill details by business bill number.</summary>
public sealed record GetBillDetailsByNumberQuery
  : IRequest<ApplicationResult<BillDetailDto>>
{
  public string BillNumber { get; init; } = string.Empty;
}
