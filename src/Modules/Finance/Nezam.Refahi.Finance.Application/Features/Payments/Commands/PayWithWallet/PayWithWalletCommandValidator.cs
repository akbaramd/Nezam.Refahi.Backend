using FluentValidation;

namespace Nezam.Refahi.Finance.Application.Features.Payments.Commands.PayWithWallet;

/// <summary>
/// Validator for PayWithWalletCommand
/// </summary>
public class PayWithWalletCommandValidator : AbstractValidator<Contracts.Commands.Payments.PayWithWalletCommand>
{
    public PayWithWalletCommandValidator()
    {
        RuleFor(x => x.PaymentId)
            .NotEmpty()
            .WithMessage("شناسه پرداخت الزامی است");

        RuleFor(x => x.BillId)
            .NotEmpty()
            .WithMessage("شناسه صورت حساب الزامی است");

        RuleFor(x => x.WalletId)
            .NotEmpty()
            .WithMessage("شناسه کیف پول الزامی است");

        RuleFor(x => x.AmountRials)
            .GreaterThan(0)
            .WithMessage("مبلغ پرداخت باید بیشتر از صفر باشد")
            .When(x => x.AmountRials.HasValue);

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("توضیحات نمی‌تواند بیش از 500 کاراکتر باشد")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.ExternalReference)
            .MaximumLength(100)
            .WithMessage("مرجع خارجی نمی‌تواند بیش از 100 کاراکتر باشد")
            .When(x => !string.IsNullOrEmpty(x.ExternalReference));
    }
}
