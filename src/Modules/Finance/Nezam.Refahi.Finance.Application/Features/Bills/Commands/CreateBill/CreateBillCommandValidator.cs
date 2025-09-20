using FluentValidation;
using Nezam.Refahi.Finance.Contracts.Commands.Bills;

namespace Nezam.Refahi.Finance.Application.Features.Bills.Commands.CreateBill;

/// <summary>
/// Validator for CreateBillCommand
/// </summary>
public class CreateBillCommandValidator : AbstractValidator<CreateBillCommand>
{
    public CreateBillCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.ReferenceId)
            .NotEmpty()
            .WithMessage("Reference ID is required")
            .MaximumLength(100)
            .WithMessage("Reference ID cannot exceed 100 characters");

        RuleFor(x => x.BillType)
            .NotEmpty()
            .WithMessage("Bill type is required")
            .MaximumLength(50)
            .WithMessage("Bill type cannot exceed 50 characters");

        RuleFor(x => x.UserNationalNumber)
            .NotEmpty()
            .WithMessage("User national number is required")
            .Length(10)
            .WithMessage("National number must be exactly 10 digits")
            .Matches(@"^\d{10}$")
            .WithMessage("National number must contain only digits");

        RuleFor(x => x.UserFullName)
            .MaximumLength(200)
            .WithMessage("User full name cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.UserFullName));

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Due date must be in the future")
            .When(x => x.DueDate.HasValue);

        RuleForEach(x => x.Items)
            .SetValidator(new CreateBillItemValidator())
            .When(x => x.Items != null);
    }
}

/// <summary>
/// Validator for CreateBillItemDto
/// </summary>
public class CreateBillItemValidator : AbstractValidator<Nezam.Refahi.Finance.Contracts.Dtos.CreateBillItemDto>
{
    public CreateBillItemValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Item title is required")
            .MaximumLength(200)
            .WithMessage("Item title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Item description cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.UnitPriceRials)
            .GreaterThan(0)
            .WithMessage("Unit price must be greater than zero");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero");

        RuleFor(x => x.DiscountPercentage)
            .InclusiveBetween(0, 100)
            .WithMessage("Discount percentage must be between 0 and 100")
            .When(x => x.DiscountPercentage.HasValue);
    }
}