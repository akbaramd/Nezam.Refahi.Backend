using FluentValidation;
using Nezam.Refahi.Finance.Contracts.Commands.Wallets;

namespace Nezam.Refahi.Finance.Application.Features.Wallets.Commands.CreateWallet;

/// <summary>
/// Validator for CreateWalletCommand
/// </summary>
public class CreateWalletCommandValidator : AbstractValidator<CreateWalletCommand>
{
    public CreateWalletCommandValidator()
    {
        RuleFor(x => x.UserNationalNumber)
            .NotEmpty()
            .WithMessage("User national number is required")
            .Length(10, 10)
            .WithMessage("National number must be exactly 10 digits")
            .Matches(@"^\d{10}$")
            .WithMessage("National number must contain only digits");

        RuleFor(x => x.UserFullName)
            .NotEmpty()
            .WithMessage("User full name is required")
            .MaximumLength(200)
            .WithMessage("User full name cannot exceed 200 characters");

        RuleFor(x => x.InitialBalanceRials)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Initial balance cannot be negative")
            .LessThan(1_000_000_000_000) // 1 trillion rials
            .WithMessage("Initial balance cannot exceed 1 trillion rials");

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
            .Must(kvp => !string.IsNullOrEmpty(kvp.Value) && kvp.Value.Length <= 200)
            .WithMessage("Metadata value cannot be empty and must not exceed 200 characters")
            .When(x => x.Metadata != null);
    }
}
