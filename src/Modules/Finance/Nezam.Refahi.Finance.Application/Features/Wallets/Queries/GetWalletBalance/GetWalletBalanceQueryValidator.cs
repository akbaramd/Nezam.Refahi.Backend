using FluentValidation;
using Nezam.Refahi.Finance.Contracts.Queries.Wallets;

namespace Nezam.Refahi.Finance.Application.Features.Wallets.Queries.GetWalletBalance;

/// <summary>
/// Validator for GetWalletBalanceQuery
/// </summary>
public class GetWalletBalanceQueryValidator : AbstractValidator<GetWalletBalanceQuery>
{
    public GetWalletBalanceQueryValidator()
    {
        RuleFor(x => x.UserNationalNumber)
            .NotEmpty()
            .WithMessage("User national number is required")
            .Length(10, 10)
            .WithMessage("National number must be exactly 10 digits")
            .Matches(@"^\d{10}$")
            .WithMessage("National number must contain only digits");

        RuleFor(x => x.TransactionHistoryCount)
            .GreaterThan(0)
            .WithMessage("Transaction history count must be greater than zero")
            .LessThanOrEqualTo(50)
            .WithMessage("Transaction history count cannot exceed 50");

        RuleFor(x => x.AnalysisDays)
            .GreaterThan(0)
            .WithMessage("Analysis days must be greater than zero")
            .LessThanOrEqualTo(365)
            .WithMessage("Analysis days cannot exceed 365");
    }
}
