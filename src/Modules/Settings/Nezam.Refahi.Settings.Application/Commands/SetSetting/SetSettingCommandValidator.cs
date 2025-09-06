using FluentValidation;

namespace Nezam.Refahi.Settings.Application.Commands.SetSetting;

/// <summary>
/// Validator for the SetSettingCommand
/// </summary>
public class SetSettingCommandValidator : AbstractValidator<SetSettingCommand>
{
    public SetSettingCommandValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty().WithMessage("Setting key is required")
            .MaximumLength(100).WithMessage("Setting key cannot exceed 100 characters")
            .MinimumLength(2).WithMessage("Setting key must be at least 2 characters")
            .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Setting key can only contain letters, numbers, and underscores");

        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Setting value is required");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid setting type");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Setting description is required")
            .MaximumLength(500).WithMessage("Setting description cannot exceed 500 characters")
            .MinimumLength(10).WithMessage("Setting description must be at least 10 characters");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display order cannot be negative")
            .LessThanOrEqualTo(10000).WithMessage("Display order cannot exceed 10000");

        RuleFor(x => x.ChangeReason)
            .MaximumLength(1000).WithMessage("Change reason cannot exceed 1000 characters");
    }
}