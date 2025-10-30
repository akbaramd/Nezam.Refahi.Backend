using MediatR;
using Nezam.Refahi.Finance.Application.DTOs;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Queries.Bills;

/// <summary>Get full bill details by external tracking code (e.g., wallet deposits).</summary>
public sealed record GetBillDetailsByTrackingCodeQuery
  : IRequest<ApplicationResult<BillDetailDto>>
{
  public string TrackingCode { get; init; } = string.Empty;

}