using FluentValidation;
using Nezam.Refahi.Finance.Application.Commands.Wallets;

namespace Nezam.Refahi.Finance.Application.Features.Wallets.Commands.ChargeWallet;

/// <summary>
/// Validator for ChargeWalletCommand
/// </summary>
public class ChargeWalletCommandValidator : AbstractValidator<ChargeWalletCommand>
{
    public ChargeWalletCommandValidator()
    {
        RuleFor(x => x.ExternalUserId)
            .NotEmpty()
            .WithMessage("User external user ID is required")
            .NotEqual(Guid.Empty)
            .WithMessage("User external user ID cannot be empty");

        RuleFor(x => x.AmountRials)
            .GreaterThan(0)
            .WithMessage("Charge amount must be greater than zero")
            .LessThanOrEqualTo(100_000_000_000) // 100 billion rials
            .WithMessage("Charge amount cannot exceed 100 billion rials");

        RuleFor(x => x.ReferenceId)
            .NotEmpty()
            .WithMessage("Reference ID is required")
            .MaximumLength(100)
            .WithMessage("Reference ID cannot exceed 100 characters");

        RuleFor(x => x.ExternalReference)
            .MaximumLength(200)
            .WithMessage("External reference cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.ExternalReference));

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Metadata)
            .Must(metadata => metadata == null || metadata.Count <= 20)
            .WithMessage("Metadata cannot contain more than 20 items")
            .When(x => x.Metadata != null);

        RuleForEach(x => x.Metadata)
            .Must(kvp => !string.IsNullOrEmpty(kvp.Key) && kvp.Key.Length <= 50)
            .WithMessage("Metadata key cannot be empty and must not exceed 50 characters")
            .When(x => x.Metadata != null);
    }
}
