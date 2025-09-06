using FluentValidation;

namespace Nezam.Refahi.Settings.Application.Commands.CreateCategory;

/// <summary>
/// Validator for the CreateCategoryCommand
/// </summary>
public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required")
            .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters")
            .MinimumLength(2).WithMessage("Category name must be at least 2 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Category description is required")
            .MaximumLength(500).WithMessage("Category description cannot exceed 500 characters")
            .MinimumLength(10).WithMessage("Category description must be at least 10 characters");

        RuleFor(x => x.SectionId)
            .NotEmpty().WithMessage("Section ID is required");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display order cannot be negative")
            .LessThanOrEqualTo(10000).WithMessage("Display order cannot exceed 10000");
    }
}
