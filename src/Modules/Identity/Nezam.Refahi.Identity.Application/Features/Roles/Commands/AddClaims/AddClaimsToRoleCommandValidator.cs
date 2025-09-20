using FluentValidation;

namespace Nezam.Refahi.Identity.Application.Features.Roles.Commands.AddClaims;

public class AddClaimsToRoleCommandValidator : AbstractValidator<AddClaimsToRoleCommand>
{
    public AddClaimsToRoleCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithMessage("Role ID is required");

        RuleFor(x => x.ClaimValues)
            .NotEmpty()
            .WithMessage("At least one claim value is required")
            .Must(claims => claims.All(c => !string.IsNullOrWhiteSpace(c)))
            .WithMessage("All claim values must be valid (not null or whitespace)");
    }
}