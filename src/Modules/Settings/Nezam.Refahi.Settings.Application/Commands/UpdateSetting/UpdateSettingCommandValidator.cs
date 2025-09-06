using FluentValidation;

namespace Nezam.Refahi.Settings.Application.Commands.UpdateSetting;

/// <summary>
/// Validator for the UpdateSettingCommand
/// </summary>
public class UpdateSettingCommandValidator : AbstractValidator<UpdateSettingCommand>
{
    public UpdateSettingCommandValidator()
    {
        RuleFor(x => x.SettingId)
            .NotEmpty().WithMessage("Setting ID is required");

        RuleFor(x => x.NewValue)
            .NotEmpty().WithMessage("New value is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.ChangeReason)
            .MaximumLength(1000).WithMessage("Change reason cannot exceed 1000 characters");
    }
}
