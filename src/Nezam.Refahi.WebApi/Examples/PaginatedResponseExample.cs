using Swashbuckle.AspNetCore.Filters;

namespace Nezam.Refahi.WebApi.Examples;

public class PaginatedResponseExample : IExamplesProvider<object>
{
  public object GetExamples()
  {
    return new
    {
      data = new[]
      {
        new { id = 1, name = "آیتم اول", createdAt = DateTime.UtcNow.AddDays(-2) },
        new { id = 2, name = "آیتم دوم", createdAt = DateTime.UtcNow.AddDays(-1) },
        new { id = 3, name = "آیتم سوم", createdAt = DateTime.UtcNow }
      },
      pagination = new
      {
        currentPage = 1,
        totalPages = 5,
        pageSize = 10,
        totalCount = 50,
        hasNext = true,
        hasPrevious = false
      }
    };
  }
}