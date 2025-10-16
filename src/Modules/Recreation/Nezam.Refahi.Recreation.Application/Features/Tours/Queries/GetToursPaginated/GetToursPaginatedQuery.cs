using MediatR;
using Nezam.Refahi.Recreation.Application.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.Tours.Queries.GetToursPaginated;

public class GetToursPaginatedQuery
  : IRequest<ApplicationResult<PaginatedResult<TourDto>>>
{
  /// <summary>صفحه مورد نظر (۱ به بالا)</summary>
  public int PageNumber { get; init; } = 1;

  /// <summary>تعداد رکورد در هر صفحه</summary>
  public int PageSize { get; init; } = 10;

  /// <summary>عبارت جستجو اختیاری (مثلاً روی عنوان تور)</summary>
  public string? Search { get; init; }

  /// <summary>فیلتر بر اساس وضعیت فعال/غیرفعال</summary>
  public bool? IsActive { get; init; }




}