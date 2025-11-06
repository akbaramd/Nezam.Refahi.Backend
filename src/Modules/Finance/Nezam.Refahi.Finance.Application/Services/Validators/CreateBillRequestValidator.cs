using FluentValidation;
using Nezam.Refahi.Finance.Contracts.Services;

namespace Nezam.Refahi.Finance.Application.Services.Validators;

/// <summary>
/// Validator for CreateBillRequest
/// </summary>
public class CreateBillRequestValidator : AbstractValidator<CreateBillRequest>
{
    public CreateBillRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("عنوان فاکتور الزامی است")
            .MaximumLength(500)
            .WithMessage("عنوان فاکتور نمی‌تواند بیشتر از 500 کاراکتر باشد");

        RuleFor(x => x.ReferenceTrackingCode)
            .NotEmpty()
            .WithMessage("کد پیگیری مرجع الزامی است")
            .MaximumLength(200)
            .WithMessage("کد پیگیری مرجع نمی‌تواند بیشتر از 200 کاراکتر باشد");

        RuleFor(x => x.ReferenceId)
            .NotEmpty()
            .WithMessage("شناسه مرجع الزامی است")
            .MaximumLength(100)
            .WithMessage("شناسه مرجع نمی‌تواند بیشتر از 100 کاراکتر باشد");

        RuleFor(x => x.BillType)
            .NotEmpty()
            .WithMessage("نوع فاکتور الزامی است")
            .MaximumLength(100)
            .WithMessage("نوع فاکتور نمی‌تواند بیشتر از 100 کاراکتر باشد");



        RuleFor(x => x.UserFullName)
            .MaximumLength(200)
            .WithMessage("نام کامل کاربر نمی‌تواند بیشتر از 200 کاراکتر باشد")
            .When(x => !string.IsNullOrWhiteSpace(x.UserFullName));

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .WithMessage("توضیحات نمی‌تواند بیشتر از 2000 کاراکتر باشد")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("تاریخ سررسید باید در آینده باشد")
            .When(x => x.DueDate.HasValue);

        RuleForEach(x => x.Items)
            .SetValidator(new BillItemRequestValidator())
            .When(x => x.Items != null && x.Items.Any());

        RuleFor(x => x.Items)
            .Must(items => items != null && items.Any())
            .WithMessage("فاکتور باید حداقل یک قلم داشته باشد");
    }
}

/// <summary>
/// Validator for BillItemRequest
/// </summary>
public class BillItemRequestValidator : AbstractValidator<BillItemRequest>
{
    public BillItemRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("عنوان قلم فاکتور الزامی است")
            .MaximumLength(500)
            .WithMessage("عنوان قلم فاکتور نمی‌تواند بیشتر از 500 کاراکتر باشد");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("توضیحات قلم فاکتور نمی‌تواند بیشتر از 1000 کاراکتر باشد")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.UnitPriceRials)
            .GreaterThanOrEqualTo(0)
            .WithMessage("قیمت واحد نمی‌تواند منفی باشد");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("تعداد باید بزرگتر از صفر باشد")
            .LessThanOrEqualTo(1000000)
            .WithMessage("تعداد نمی‌تواند بیشتر از 1000000 باشد");

        RuleFor(x => x.DiscountPercentage)
            .InclusiveBetween(0, 100)
            .WithMessage("درصد تخفیف باید بین 0 تا 100 باشد")
            .When(x => x.DiscountPercentage.HasValue);
    }
}

