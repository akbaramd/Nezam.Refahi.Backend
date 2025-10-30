using MediatR;
using Nezam.Refahi.Finance.Application.DTOs;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Features.Payments.Queries.GetPaymentDetail;

public sealed partial class GetPaymentDetailQuery : IRequest<ApplicationResult<PaymentDetailDto>>
{
  /// <summary>
  /// Payment ID (required)
  /// </summary>
  public Guid PaymentId { get; init; }
}