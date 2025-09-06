using FluentValidation;

namespace Nezam.Refahi.Settings.Application.Commands.CreateSection;

/// <summary>
/// Validator for the CreateSectionCommand
/// </summary>
public class CreateSectionCommandValidator : AbstractValidator<CreateSectionCommand>
{
    public CreateSectionCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Section name is required")
            .MaximumLength(100).WithMessage("Section name cannot exceed 100 characters")
            .MinimumLength(2).WithMessage("Section name must be at least 2 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Section description is required")
            .MaximumLength(500).WithMessage("Section description cannot exceed 500 characters")
            .MinimumLength(10).WithMessage("Section description must be at least 10 characters");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display order cannot be negative")
            .LessThanOrEqualTo(10000).WithMessage("Display order cannot exceed 10000");
    }
}
