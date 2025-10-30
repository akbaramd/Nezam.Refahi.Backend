using FluentValidation;

namespace Nezam.Refahi.Finance.Application.Features.Payments.Queries.GetPaymentDetail;

/// <summary>
/// Validator for GetPaymentDetailQuery
/// </summary>
public sealed class GetPaymentDetailQueryValidator : AbstractValidator<GetPaymentDetailQuery>
{
  public GetPaymentDetailQueryValidator()
  {
    RuleFor(x => x.PaymentId)
      .NotEmpty()
      .WithMessage("شناسه پرداخت الزامی است.")
      .NotEqual(Guid.Empty)
      .WithMessage("شناسه پرداخت معتبر نیست.");
  }
}
