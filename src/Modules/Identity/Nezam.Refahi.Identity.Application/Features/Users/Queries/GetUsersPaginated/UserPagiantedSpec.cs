using MCA.SharedKernel.Domain.Contracts.Specifications;
using Nezam.Refahi.Identity.Domain.Entities;

namespace Nezam.Refahi.Identity.Application.Features.Users.Queries.GetUsersPaginated;

public class UserPagiantedSpec : IPaginatedSpecification<User>
{
  public UserPagiantedSpec(int pageNumber, int pageSize, string? search)
  {
    if (pageNumber <= 0) throw new ArgumentOutOfRangeException(nameof(pageNumber));
    if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));
    PageNumber = pageNumber;
    PageSize = pageSize;
    Search = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
  }

  public IQueryable<User> Apply(IQueryable<User> query)
  {
    // نگه‌داشتن IQueryable برای اجرا در لایه Infrastructure/EF
    var q = query;

    // Search normalization
    if (!string.IsNullOrWhiteSpace(Search))
    {
      var term = Search!.Trim().ToLower();
      q = q.Where(u =>
        (u.FirstName != null && u.FirstName.ToLower().Contains(term)) ||
        (u.LastName != null && u.LastName.ToLower().Contains(term)) ||
        (u.Username != null && u.Username.ToLower().Contains(term)) ||
        (u.Email != null && u.Email.ToLower().Contains(term)) ||
        (u.NationalId != null && u.NationalId.Value.ToLower().Contains(term)) ||
        (u.PhoneNumber != null && u.PhoneNumber.Value.ToLower().Contains(term))
      );
    }

    // سورت پیش‌فرض: جدیدترین ایجاد/شناسه (در نبود CreatedAt در موجودیت، از Id استفاده می‌کنیم)
    q = q.OrderByDescending(u => u.Id);

  
    return q;
  }

  public int PageNumber { get; }
  public int PageSize { get; }
  public string? Search { get; }
}