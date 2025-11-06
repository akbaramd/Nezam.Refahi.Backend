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

  /// <summary>
  /// External User ID to ensure user can only access their own payments
  /// </summary>
  public Guid ExternalUserId { get; init; }
}