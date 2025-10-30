using FluentValidation;
using Nezam.Refahi.Finance.Application.Features.Payments.Queries.GetBillPayments;

namespace Nezam.Refahi.Finance.Application.Features.Payments.Validators;

/// <summary>
/// Validator for GetBillPaymentsQuery
/// </summary>
public sealed class GetBillPaymentsQueryValidator : AbstractValidator<GetBillPaymentsQuery>
{
  private static readonly string[] ValidSortFields = { "id", "issuedate", "duedate", "amount", "status" };
  private static readonly string[] ValidSortDirections = { "asc", "desc" };

  public GetBillPaymentsQueryValidator()
  {
    RuleFor(x => x.BillId)
      .NotEmpty()
      .WithMessage("شناسه صورتحساب (BillId) الزامی است.");

    RuleFor(x => x.PageNumber)
      .GreaterThan(0)
      .WithMessage("شماره صفحه باید بزرگتر از صفر باشد.");

    RuleFor(x => x.PageSize)
      .InclusiveBetween(1, 100)
      .WithMessage("تعداد آیتم در هر صفحه باید بین 1 تا 100 باشد.");

    RuleFor(x => x.SortBy)
      .Must(field => ValidSortFields.Contains(field.ToLowerInvariant()))
      .WithMessage($"فیلد مرتب‌سازی نامعتبر است. فیلدهای مجاز: {string.Join(", ", ValidSortFields)}");

    RuleFor(x => x.SortDirection)
      .Must(dir => ValidSortDirections.Contains(dir.ToLowerInvariant()))
      .WithMessage($"جهت مرتب‌سازی نامعتبر است. مقادیر مجاز: asc, desc");
  }
}
