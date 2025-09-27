using FluentValidation;
using Nezam.Refahi.Finance.Application.Commands.Refunds;

namespace Nezam.Refahi.Finance.Application.Features.Refunds.Commands.CreateRefund;

/// <summary>
/// Validator for CreateRefundCommand
/// </summary>
public class CreateRefundCommandValidator : AbstractValidator<CreateRefundCommand>
{
    public CreateRefundCommandValidator()
    {
        RuleFor(x => x.BillId)
            .NotEmpty()
            .WithMessage("Bill ID is required");

        RuleFor(x => x.RefundAmountRials)
            .GreaterThan(0)
            .WithMessage("Refund amount must be greater than zero");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Refund reason is required")
            .MaximumLength(1000)
            .WithMessage("Refund reason cannot exceed 1000 characters");

        RuleFor(x => x.RequestedByExternalUserId)
            .NotEqual(Guid.Empty)
            .WithMessage("User external user ID cannot be empty");

    }
}
