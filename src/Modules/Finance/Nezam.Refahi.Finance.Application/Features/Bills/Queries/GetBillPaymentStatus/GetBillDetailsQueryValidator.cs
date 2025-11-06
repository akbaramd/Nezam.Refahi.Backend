using FluentValidation;
using Nezam.Refahi.Finance.Application.Queries.Bills;
using MCA.SharedKernel.Application.Contracts;
using MCA.SharedKernel.Domain.Models;
namespace Nezam.Refahi.Finance.Application.Features.Bills.Queries.GetBillPaymentStatus;

/// <summary>
/// Validator for GetBillPaymentStatusQuery
/// </summary>

public sealed class GetBillDetailsByIdQueryValidator : AbstractValidator<GetBillDetailsByIdQuery>
{
  public GetBillDetailsByIdQueryValidator()
  {
    RuleFor(x => x.BillId)
      .NotEmpty().WithMessage("شناسه صورت حساب الزامی است.")
      .NotEqual(Guid.Empty).WithMessage("شناسه صورت حساب معتبر نیست.");

    RuleFor(x => x.ExternalUserId)
      .NotEmpty().WithMessage("شناسه کاربر خارجی الزامی است.")
      .NotEqual(Guid.Empty).WithMessage("شناسه کاربر خارجی معتبر نیست.");
  }
}

public sealed class GetBillDetailsByNumberQueryValidator : AbstractValidator<GetBillDetailsByNumberQuery>
{
  public GetBillDetailsByNumberQueryValidator()
  {
    RuleFor(x => x.BillNumber)
      .NotEmpty().WithMessage("BillNumber is required.")
      .MaximumLength(128).WithMessage("BillNumber is too long.");
  }
}

public sealed class GetBillDetailsByTrackingCodeQueryValidator : AbstractValidator<GetBillDetailsByTrackingCodeQuery>
{
  public GetBillDetailsByTrackingCodeQueryValidator()
  {
    RuleFor(x => x.TrackingCode)
      .NotEmpty().WithMessage("TrackingCode is required.")
      .MaximumLength(128).WithMessage("TrackingCode is too long.");

  }
}