using FluentValidation;

namespace Nezam.Refahi.Finance.Application.DiscountCodes.Validation;

public sealed class ValidateDiscountCodeQueryValidator : AbstractValidator<ValidateDiscountCodeQuery>
{
  public ValidateDiscountCodeQueryValidator()
  {
    RuleFor(x => x.BillId)
      .NotEmpty().WithMessage("BillId is required.");

    RuleFor(x => x.ExternalUserId)
      .NotEmpty().WithMessage("ExternalUserId is required.");

    RuleFor(x => x.DiscountCode)
      .NotEmpty().WithMessage("DiscountCode is required.")
      .MinimumLength(2).WithMessage("DiscountCode is too short.")
      .MaximumLength(64).WithMessage("DiscountCode is too long.")
      .Matches("^[A-Za-z0-9_-]+$")
      .WithMessage("DiscountCode contains invalid characters. Only A-Z, a-z, 0-9, underscore, and hyphen are allowed.");
  }
}
