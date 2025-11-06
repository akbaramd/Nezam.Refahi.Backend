using MCA.SharedKernel.Domain.Models;
using MediatR;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Queries.GetReservationsPaginated;

/// <summary>
/// Query to get paginated list of reservations for the current user
/// </summary>
public class GetReservationsPaginatedQuery : IRequest<ApplicationResult<PaginatedResult<ReservationDto>>>
{
    /// <summary>صفحه مورد نظر (۱ به بالا)</summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>تعداد رکورد در هر صفحه</summary>
    public int PageSize { get; init; } = 10;

    /// <summary>فیلتر بر اساس وضعیت رزرو (اختیاری)</summary>
    public ReservationStatus? Status { get; init; }

    /// <summary>عبارت جستجو اختیاری (مثلاً روی کد رهگیری یا عنوان تور)</summary>
    public string? Search { get; init; }

    /// <summary>فیلتر از تاریخ رزرو (اختیاری)</summary>
    public DateTime? FromDate { get; init; }

    /// <summary>فیلتر تا تاریخ رزرو (اختیاری)</summary>
    public DateTime? ToDate { get; init; }

    /// <summary>شناسه کاربر خارجی (اختیاری - در صورت عدم ارسال، از ICurrentUserService استفاده می‌شود)</summary>
    public Guid? ExternalUserId { get; init; }
}

