using MCA.SharedKernel.Domain.Models;
using MediatR;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.Tours.Queries.GetToursPaginated;

public class GetToursPaginatedQuery
  : IRequest<ApplicationResult<PaginatedResult<TourWithUserReservationDto>>>
{
  /// <summary>صفحه مورد نظر (۱ به بالا)</summary>
  public int PageNumber { get; init; } = 1;

  /// <summary>تعداد رکورد در هر صفحه</summary>
  public int PageSize { get; init; } = 10;

  /// <summary>عبارت جستجو اختیاری (مثلاً روی عنوان تور)</summary>
  public string? Search { get; init; }

  /// <summary>فیلتر بر اساس وضعیت فعال/غیرفعال</summary>
  public bool? IsActive { get; init; }

  /// <summary>Optional external user id to enrich with user's reservation context</summary>
  public Guid? ExternalUserId { get; init; }
}