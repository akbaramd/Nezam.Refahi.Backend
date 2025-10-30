using MediatR;
using Nezam.Refahi.Finance.Application.DTOs;
using Nezam.Refahi.Shared.Application.Common.Models;

public sealed record ValidateDiscountCodeQuery : IRequest<ApplicationResult<DiscountValidationDto>>
{
  public Guid BillId { get; init; }
  public string DiscountCode { get; init; } = string.Empty;
  public Guid ExternalUserId { get; init; }
}
