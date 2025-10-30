using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Finance.Application.DTOs;
using Nezam.Refahi.Finance.Domain.Entities;

namespace Nezam.Refahi.Finance.Application.Mappers;

public sealed class BillItemMapper : IMapper<BillItem, BillItemDto>
{
  public Task<BillItemDto> MapAsync(BillItem source, CancellationToken cancellationToken = default)
  {
    cancellationToken.ThrowIfCancellationRequested();
    if (source is null) throw new ArgumentNullException(nameof(source));
    return Task.FromResult(BillItemDto.FromEntity(source));
  }

  public Task MapAsync(BillItem source, BillItemDto destination, CancellationToken cancellationToken = default)
  {
    cancellationToken.ThrowIfCancellationRequested();
    if (source is null) throw new ArgumentNullException(nameof(source));
    if (destination is null) throw new ArgumentNullException(nameof(destination));

    var dto = BillItemDto.FromEntity(source);
    CopyInto(dto, destination);
    return Task.CompletedTask;
  }

  private static void CopyInto(BillItemDto src, BillItemDto dst)
  {
    dst.ItemId            = src.ItemId;
    dst.Title             = src.Title;
    dst.Description       = src.Description;
    dst.UnitPriceRials    = src.UnitPriceRials;
    dst.Quantity          = src.Quantity;
    dst.DiscountPercentage= src.DiscountPercentage;
    dst.LineTotalRials    = src.LineTotalRials;
    dst.CreatedAt         = src.CreatedAt;
  }
}