using FluentValidation;
using Nezam.Refahi.Finance.Application.Commands.Bills;

namespace Nezam.Refahi.Finance.Application.Features.Bills.Commands.IssueBill;

/// <summary>
/// Validator for IssueBillCommand
/// </summary>
public class IssueBillCommandValidator : AbstractValidator<IssueBillCommand>
{
    public IssueBillCommandValidator()
    {
        RuleFor(x => x.BillId)
            .NotEmpty()
            .WithMessage("Bill ID is required");
    }
}
