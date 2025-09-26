using FluentValidation;
using Nezam.Refahi.Finance.Contracts.Queries.Wallets;

namespace Nezam.Refahi.Finance.Application.Features.Wallets.Queries.GetWalletDeposits;

/// <summary>
/// Validator for GetWalletDepositsQuery
/// </summary>
public class GetWalletDepositsQueryValidator : AbstractValidator<GetWalletDepositsQuery>
{
    public GetWalletDepositsQueryValidator()
    {
        RuleFor(x => x.ExternalUserId)  
            .NotEmpty()
            .WithMessage("External user ID is required");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100)
            .WithMessage("Page size cannot exceed 100");

        RuleFor(x => x.Status)
            .Must(BeValidStatus)
            .When(x => !string.IsNullOrEmpty(x.Status))
            .WithMessage("Invalid status value");

        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate)
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue)
            .WithMessage("From date must be less than or equal to to date");
    }

    private static bool BeValidStatus(string? status)
    {
        if (string.IsNullOrEmpty(status))
            return true;

        return Enum.TryParse<Domain.Enums.WalletDepositStatus>(status, true, out _);
    }
}
