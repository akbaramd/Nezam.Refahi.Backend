using MCA.SharedKernel.Application.Contracts;
using MCA.SharedKernel.Domain.Models;
using MediatR;
using Nezam.Refahi.Finance.Application.DTOs;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Features.Payments.Queries.GetBillPayments;

public class GetBillPaymentsQuery : IRequest<ApplicationResult<PaginatedResult<PaymentDto>>>
{


  /// <summary>
  /// Filter by bill type (optional)
  /// </summary>
  public Guid BillId { get; init; }


  /// <summary>
  /// Filter by bill type (optional)
  /// </summary>
  public string? SearchTerm { get; init; }
  /// <summary>
  /// Page number for pagination (default: 1)
  /// </summary>
  public int PageNumber { get; init; } = 1;

  /// <summary>
  /// Page size for pagination (default: 20, max: 100)
  /// </summary>
  public int PageSize { get; init; } = 20;

  /// <summary>
  /// Sort by field (IssueDate, DueDate, TotalAmount, Status)
  /// </summary>
  public string SortBy { get; init; } = "Id";

  /// <summary>
  /// Sort direction (asc, desc)
  /// </summary>
  public string SortDirection { get; init; } = "desc";
}
