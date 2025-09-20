using FluentValidation;

namespace Nezam.Refahi.Identity.Application.Features.Roles.Queries.GetRoleById;

public class GetRoleByIdQueryValidator : AbstractValidator<GetRoleByIdQuery>
{
    public GetRoleByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Role ID is required");
    }
}