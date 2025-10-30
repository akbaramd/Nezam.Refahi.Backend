using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Finance.Application.DTOs;
using Nezam.Refahi.Finance.Domain.Entities;

namespace Nezam.Refahi.Finance.Application.Mappers;

public sealed class PaymentTransactionMapper : IMapper<PaymentTransaction, PaymentTransactionDto>
{
  public Task<PaymentTransactionDto> MapAsync(PaymentTransaction source, CancellationToken cancellationToken = default)
  {
    cancellationToken.ThrowIfCancellationRequested();
    if (source is null) throw new ArgumentNullException(nameof(source));
    return Task.FromResult(PaymentTransactionDto.FromEntity(source));
  }

  public Task MapAsync(PaymentTransaction source, PaymentTransactionDto destination, CancellationToken cancellationToken = default)
  {
    cancellationToken.ThrowIfCancellationRequested();
    if (source is null) throw new ArgumentNullException(nameof(source));
    if (destination is null) throw new ArgumentNullException(nameof(destination));

    var dto = PaymentTransactionDto.FromEntity(source);
    CopyInto(dto, destination);
    return Task.CompletedTask;
  }

  private static void CopyInto(PaymentTransactionDto src, PaymentTransactionDto dst)
  {
    dst.TransactionId        = src.TransactionId;
    dst.Status               = src.Status;
    dst.AmountRials          = src.AmountRials;
    dst.CreatedAt            = src.CreatedAt;
    dst.Gateway              = src.Gateway;
    dst.GatewayTransactionId = src.GatewayTransactionId;
    dst.GatewayReference     = src.GatewayReference;
    dst.Note                 = src.Note;
  }
}
