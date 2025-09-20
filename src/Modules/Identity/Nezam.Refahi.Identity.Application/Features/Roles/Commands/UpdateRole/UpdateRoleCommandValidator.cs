using FluentValidation;

namespace Nezam.Refahi.Identity.Application.Features.Roles.Commands.UpdateRole;

public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Role ID is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Role name is required")
            .MaximumLength(100)
            .WithMessage("Role name cannot exceed 100 characters")
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("Role name cannot be empty or whitespace only");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Display order must be non-negative");

       
    }
}