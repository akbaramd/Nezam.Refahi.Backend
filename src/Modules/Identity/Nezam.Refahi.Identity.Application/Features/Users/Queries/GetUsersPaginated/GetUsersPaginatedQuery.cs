using MediatR;
using Nezam.Refahi.Identity.Domain.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Features.Users.Queries.GetUsersPaginated;

public class GetUsersPaginatedQuery 
  : IRequest<ApplicationResult<PaginatedResult<UserDto>>>
{
  /// <summary>صفحه مورد نظر (۱ به بالا)</summary>
  public int PageNumber { get; init; } = 1;

  /// <summary>تعداد رکورد در هر صفحه</summary>
  public int PageSize{ get; init; } = 10;

  /// <summary>عبارت جستجو اختیاری (مثلاً روی نام/شماره موبایل)</summary>
  public string? Search { get; init; }
}