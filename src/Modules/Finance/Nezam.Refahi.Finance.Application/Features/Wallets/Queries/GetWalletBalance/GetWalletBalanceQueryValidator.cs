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
        RuleFor(x => x.ExternalUserId)
            .NotEmpty()
            .WithMessage("شناسه کاربر الزامی است")
            .NotEqual(Guid.Empty)
            .WithMessage("شناسه کاربر نمی‌تواند خالی باشد");

     

        RuleFor(x => x.TransactionHistoryCount)
            .GreaterThan(0)
            .WithMessage("تعداد تراکنش‌های تاریخچه باید بیشتر از صفر باشد")
            .LessThanOrEqualTo(50)
            .WithMessage("تعداد تراکنش‌های تاریخچه نمی‌تواند بیش از 50 باشد");

        RuleFor(x => x.AnalysisDays)
            .GreaterThan(0)
            .WithMessage("تعداد روزهای تحلیل باید بیشتر از صفر باشد")
            .LessThanOrEqualTo(365)
            .WithMessage("تعداد روزهای تحلیل نمی‌تواند بیش از 365 روز باشد");
    }
}
