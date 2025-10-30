using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Finance.Application.DTOs;
using Nezam.Refahi.Finance.Domain.Entities;

namespace Nezam.Refahi.Finance.Application.Mappers;

public sealed class RefundMapper : IMapper<Refund, RefundDto>
{
  public Task<RefundDto> MapAsync(Refund source, CancellationToken cancellationToken = default)
  {
    cancellationToken.ThrowIfCancellationRequested();
    if (source is null) throw new ArgumentNullException(nameof(source));
    var dto = RefundDto.FromEntity(source, nowUtc: DateTime.UtcNow);
    return Task.FromResult(dto);
  }

  public Task MapAsync(Refund source, RefundDto destination, CancellationToken cancellationToken = default)
  {
    cancellationToken.ThrowIfCancellationRequested();
    if (source is null) throw new ArgumentNullException(nameof(source));
    if (destination is null) throw new ArgumentNullException(nameof(destination));

    var dto = RefundDto.FromEntity(source, nowUtc: DateTime.UtcNow);
    CopyInto(dto, destination);
    return Task.CompletedTask;
  }

  private static void CopyInto(RefundDto src, RefundDto dst)
  {
    dst.RefundId              = src.RefundId;
    dst.BillId                = src.BillId;
    dst.AmountRials           = src.AmountRials;
    dst.Status                = src.Status;
    dst.StatusText            = src.StatusText;
    dst.Reason                = src.Reason;
    dst.RequestedByExternalUserId = src.RequestedByExternalUserId;
    dst.RequestedAt           = src.RequestedAt;
    dst.ProcessedAt           = src.ProcessedAt;
    dst.CompletedAt           = src.CompletedAt;
    dst.GatewayRefundId       = src.GatewayRefundId;
    dst.GatewayReference      = src.GatewayReference;
    dst.ProcessorNotes        = src.ProcessorNotes;
    dst.RejectionReason       = src.RejectionReason;
    dst.SecondsSinceRequested = src.SecondsSinceRequested;
  }
}