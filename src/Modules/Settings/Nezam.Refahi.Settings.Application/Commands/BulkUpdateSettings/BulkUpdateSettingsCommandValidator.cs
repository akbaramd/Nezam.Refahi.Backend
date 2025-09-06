using FluentValidation;

namespace Nezam.Refahi.Settings.Application.Commands.BulkUpdateSettings;

/// <summary>
/// Validator for the BulkUpdateSettingsCommand
/// </summary>
public class BulkUpdateSettingsCommandValidator : AbstractValidator<BulkUpdateSettingsCommand>
{
    public BulkUpdateSettingsCommandValidator()
    {
        RuleFor(x => x.SettingUpdates)
            .NotEmpty().WithMessage("Setting updates cannot be empty")
            .Must(x => x.Count <= 100).WithMessage("Cannot update more than 100 settings at once");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.ChangeReason)
            .MaximumLength(1000).WithMessage("Change reason cannot exceed 1000 characters");

        RuleForEach(x => x.SettingUpdates)
            .Must(x => !string.IsNullOrWhiteSpace(x.Value))
            .WithMessage("Setting value cannot be empty");

        RuleForEach(x => x.SettingUpdates)
            .Must(x => x.Key != Guid.Empty)
            .WithMessage("Setting ID cannot be empty");
    }
}
