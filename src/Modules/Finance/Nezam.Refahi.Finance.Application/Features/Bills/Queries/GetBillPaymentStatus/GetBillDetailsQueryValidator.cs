using FluentValidation;
using Nezam.Refahi.Finance.Application.Queries.Bills;

namespace Nezam.Refahi.Finance.Application.Features.Bills.Queries.GetBillPaymentStatus;

/// <summary>
/// Validator for GetBillPaymentStatusQuery
/// </summary>

public sealed class GetBillDetailsByIdQueryValidator : AbstractValidator<GetBillDetailsByIdQuery>
{
  public GetBillDetailsByIdQueryValidator()
  {
    RuleFor(x => x.BillId)
      .NotEmpty().WithMessage("BillId is required.");
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

    RuleFor(x => x.BillType)
      .NotEmpty().WithMessage("BillType is required.")
      .MaximumLength(64).WithMessage("BillType is too long.");
  }
}