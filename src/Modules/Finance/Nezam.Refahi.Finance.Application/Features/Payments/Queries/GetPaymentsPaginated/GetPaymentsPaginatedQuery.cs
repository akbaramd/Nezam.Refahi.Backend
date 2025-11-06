using MCA.SharedKernel.Domain.Models;
using MediatR;
using Nezam.Refahi.Finance.Application.DTOs;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Features.Payments.Queries.GetPaymentsPaginated;

/// <summary>
/// Query to get paginated list of payments for the current user.
/// </summary>
public class GetPaymentsPaginatedQuery : IRequest<ApplicationResult<PaginatedResult<PaymentDto>>>
{
    /// <summary>صفحه مورد نظر (۱ به بالا)</summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>تعداد رکورد در هر صفحه</summary>
    public int PageSize { get; init; } = 10;

    /// <summary>فیلتر بر اساس وضعیت پرداخت (اختیاری)</summary>
    public PaymentStatus? Status { get; init; }

    /// <summary>عبارت جستجو اختیاری (مثلاً روی کد رهگیری، شماره فاکتور)</summary>
    public string? Search { get; init; }

    /// <summary>فیلتر از تاریخ ایجاد پرداخت (اختیاری)</summary>
    public DateTime? FromDate { get; init; }

    /// <summary>فیلتر تا تاریخ ایجاد پرداخت (اختیاری)</summary>
    public DateTime? ToDate { get; init; }

    /// <summary>شناسه کاربر خارجی (اختیاری - در صورت عدم ارسال، از ICurrentUserService استفاده می‌شود)</summary>
    public Guid? ExternalUserId { get; init; }
}

