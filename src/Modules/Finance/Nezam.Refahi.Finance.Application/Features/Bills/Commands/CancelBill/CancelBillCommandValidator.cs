using FluentValidation;
using Nezam.Refahi.Finance.Contracts.Commands.Bills;

namespace Nezam.Refahi.Finance.Application.Features.Bills.Commands.CancelBill;

/// <summary>
/// Validator for CancelBillCommand
/// </summary>
public class CancelBillCommandValidator : AbstractValidator<CancelBillCommand>
{
    public CancelBillCommandValidator()
    {
        RuleFor(x => x.BillId)
            .NotEmpty()
            .WithMessage("Bill ID is required");

        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .WithMessage("Cancellation reason cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Reason));
    }
}