using FluentValidation;

namespace Nezam.Refahi.Identity.Application.Features.Roles.Commands.DeleteRole;

public class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommand>
{
    public DeleteRoleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Role ID is required");
    }
}