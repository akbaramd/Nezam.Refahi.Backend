using FluentValidation;
using Nezam.Refahi.Finance.Application.Queries.Wallets;

namespace Nezam.Refahi.Finance.Application.Features.Wallets.Queries.GetUserWalletBalance;

/// <summary>
/// Validator for GetUserWalletBalance
/// </summary>
public class GetUserWalletBalanceQueryValidator : AbstractValidator<GetUserWalletBalanceQuery>
{
    public GetUserWalletBalanceQueryValidator()
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
