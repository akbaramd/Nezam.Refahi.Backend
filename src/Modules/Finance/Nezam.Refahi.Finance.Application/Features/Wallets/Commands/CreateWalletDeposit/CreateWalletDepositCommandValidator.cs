using FluentValidation;
using Nezam.Refahi.Finance.Contracts.Commands.Wallets;

namespace Nezam.Refahi.Finance.Application.Features.Wallets.Commands.CreateWalletDeposit;

/// <summary>
/// Validator for CreateWalletDepositCommand
/// </summary>
public class CreateWalletDepositCommandValidator : AbstractValidator<CreateWalletDepositCommand>
{
    public CreateWalletDepositCommandValidator()
    {
        RuleFor(x => x.ExternalUserId)
            .NotEmpty()
            .WithMessage("شماره ملی کاربر الزامی است")
            .NotEqual(Guid.Empty)
            .WithMessage("User external user ID cannot be empty");


        RuleFor(x => x.UserFullName)
            .NotEmpty()
            .WithMessage("نام کامل کاربر الزامی است")
            .MaximumLength(200)
            .WithMessage("نام کامل کاربر نمی‌تواند بیش از 200 کاراکتر باشد");

        RuleFor(x => x.AmountRials)
            .GreaterThan(0)
            .WithMessage("مبلغ واریز باید بیشتر از صفر باشد")
            .LessThanOrEqualTo(100_000_000_000) // 100 billion rials
            .WithMessage("مبلغ واریز نمی‌تواند بیش از 100 میلیارد ریال باشد");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("توضیحات نمی‌تواند بیش از 500 کاراکتر باشد")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.ExternalReference)
            .MaximumLength(200)
            .WithMessage("مرجع خارجی نمی‌تواند بیش از 200 کاراکتر باشد")
            .When(x => !string.IsNullOrEmpty(x.ExternalReference));

        RuleFor(x => x.Metadata)
            .Must(metadata => metadata == null || metadata.Count <= 20)
            .WithMessage("متادیتا نمی‌تواند بیش از 20 آیتم داشته باشد")
            .When(x => x.Metadata != null);

        RuleForEach(x => x.Metadata)
            .Must(kvp => !string.IsNullOrEmpty(kvp.Key) && kvp.Key.Length <= 50)
            .WithMessage("کلید متادیتا نمی‌تواند خالی باشد و نباید بیش از 50 کاراکتر باشد")
            .Must(kvp => !string.IsNullOrEmpty(kvp.Value) && kvp.Value.Length <= 200)
            .WithMessage("مقدار متادیتا نمی‌تواند خالی باشد و نباید بیش از 200 کاراکتر باشد")
            .When(x => x.Metadata != null);
    }
}
