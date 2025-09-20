using FluentValidation;
using Nezam.Refahi.Finance.Contracts.Commands.Bills;

namespace Nezam.Refahi.Finance.Application.Features.Bills.Commands.AddBillItem;

/// <summary>
/// Validator for AddBillItemCommand
/// </summary>
public class AddBillItemCommandValidator : AbstractValidator<AddBillItemCommand>
{
    public AddBillItemCommandValidator()
    {
        RuleFor(x => x.BillId)
            .NotEmpty()
            .WithMessage("Bill ID is required");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Item title is required")
            .MaximumLength(200)
            .WithMessage("Item title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Item description cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.UnitPriceRials)
            .GreaterThan(0)
            .WithMessage("Unit price must be greater than zero");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero");

        RuleFor(x => x.DiscountPercentage)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Discount percentage cannot be negative")
            .LessThanOrEqualTo(100)
            .WithMessage("Discount percentage cannot exceed 100")
            .When(x => x.DiscountPercentage.HasValue);
    }
}