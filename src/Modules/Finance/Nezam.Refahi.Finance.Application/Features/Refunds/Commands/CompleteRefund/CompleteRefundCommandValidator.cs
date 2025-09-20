using FluentValidation;
using Nezam.Refahi.Finance.Contracts.Commands.Refunds;

namespace Nezam.Refahi.Finance.Application.Features.Refunds.Commands.CompleteRefund;

/// <summary>
/// Validator for CompleteRefundCommand
/// </summary>
public class CompleteRefundCommandValidator : AbstractValidator<CompleteRefundCommand>
{
    public CompleteRefundCommandValidator()
    {
        RuleFor(x => x.RefundId)
            .NotEmpty()
            .WithMessage("Refund ID is required");

        RuleFor(x => x.GatewayRefundId)
            .MaximumLength(200)
            .WithMessage("Gateway refund ID cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.GatewayRefundId));

        RuleFor(x => x.GatewayReference)
            .MaximumLength(200)
            .WithMessage("Gateway reference cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.GatewayReference));
    }
}