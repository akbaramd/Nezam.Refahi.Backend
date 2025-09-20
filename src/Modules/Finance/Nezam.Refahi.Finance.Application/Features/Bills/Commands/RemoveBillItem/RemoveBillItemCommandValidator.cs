using FluentValidation;
using Nezam.Refahi.Finance.Contracts.Commands.Bills;

namespace Nezam.Refahi.Finance.Application.Features.Bills.Commands.RemoveBillItem;

/// <summary>
/// Validator for RemoveBillItemCommand
/// </summary>
public class RemoveBillItemCommandValidator : AbstractValidator<RemoveBillItemCommand>
{
    public RemoveBillItemCommandValidator()
    {
        RuleFor(x => x.BillId)
            .NotEmpty()
            .WithMessage("Bill ID is required");

        RuleFor(x => x.ItemId)
            .NotEmpty()
            .WithMessage("Item ID is required");
    }
}