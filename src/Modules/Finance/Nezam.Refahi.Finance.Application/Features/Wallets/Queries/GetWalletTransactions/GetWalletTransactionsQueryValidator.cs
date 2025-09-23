using FluentValidation;
using Nezam.Refahi.Finance.Contracts.Queries.Wallets;
using Nezam.Refahi.Finance.Domain.Enums;

namespace Nezam.Refahi.Finance.Application.Features.Wallets.Queries.GetWalletTransactions;

/// <summary>
/// Validator for GetWalletTransactionsQuery
/// </summary>
public class GetWalletTransactionsQueryValidator : AbstractValidator<GetWalletTransactionsQuery>
{
    public GetWalletTransactionsQueryValidator()
    {
        RuleFor(x => x.UserNationalNumber)
            .NotEmpty()
            .WithMessage("User national number is required")
            .Length(10, 10)
            .WithMessage("National number must be exactly 10 digits")
            .Matches(@"^\d{10}$")
            .WithMessage("National number must contain only digits");

        RuleFor(x => x.TransactionType)
            .Must(type => string.IsNullOrEmpty(type) || Enum.IsDefined(typeof(WalletTransactionType), type))
            .WithMessage("Invalid transaction type")
            .When(x => !string.IsNullOrEmpty(x.TransactionType));

        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate)
            .WithMessage("From date must be less than or equal to to date")
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue);

        RuleFor(x => x.ToDate)
            .GreaterThanOrEqualTo(x => x.FromDate)
            .WithMessage("To date must be greater than or equal to from date")
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue);

        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than zero");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than zero")
            .LessThanOrEqualTo(100)
            .WithMessage("Page size cannot exceed 100");

        RuleFor(x => x.SortBy)
            .Must(sortBy => new[] { "CreatedAt", "Amount", "TransactionType" }.Contains(sortBy))
            .WithMessage("Sort by field must be one of: CreatedAt, Amount, TransactionType");

        RuleFor(x => x.SortDirection)
            .Must(direction => new[] { "asc", "desc" }.Contains(direction.ToLower()))
            .WithMessage("Sort direction must be 'asc' or 'desc'");

        RuleFor(x => x.ReferenceId)
            .MaximumLength(100)
            .WithMessage("Reference ID cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.ReferenceId));

        RuleFor(x => x.ExternalReference)
            .MaximumLength(200)
            .WithMessage("External reference cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.ExternalReference));
    }
}
