using MediatR;
using Nezam.Refahi.Recreation.Application.Dtos;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Queries.GetUserReservations;

public class GetUserReservationsQuery : IRequest<ApplicationResult<PaginatedResult<UserReservationDto>>>
{
    /// <summary>صفحه مورد نظر (۱ به بالا)</summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>تعداد رکورد در هر صفحه</summary>
    public int PageSize { get; init; } = 10;

    /// <summary>فیلتر بر اساس وضعیت رزرو</summary>
    public ReservationStatus? Status { get; init; }

    /// <summary>جستجو بر اساس کد پیگیری</summary>
    public string? TrackingCode { get; init; }

    /// <summary>فیلتر بر اساس تاریخ رزرو (از تاریخ)</summary>
    public DateTime? FromDate { get; init; }

    /// <summary>فیلتر بر اساس تاریخ رزرو (تا تاریخ)</summary>
    public DateTime? ToDate { get; init; }
}
