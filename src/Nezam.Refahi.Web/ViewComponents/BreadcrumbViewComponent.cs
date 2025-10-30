using Microsoft.AspNetCore.Mvc;

namespace Nezam.New.EES.ViewComponents;

  public class BreadcrumbViewComponent : ViewComponent
  {
      public IViewComponentResult Invoke(string currentPage, List<BreadcrumbItem>? items = null)
      {
          var model = new BreadcrumbViewModel
          {
              CurrentPage = currentPage,
              Items = items ?? new List<BreadcrumbItem>()
          };

          return View(model);
      }
  }

  public class BreadcrumbViewModel
  {
      public string CurrentPage { get; set; } = null!;
      public List<BreadcrumbItem> Items { get; set; } = null!;
  }

  public class BreadcrumbItem
  {
      public string Title { get; set; } = null!;
      public string? Url { get; set; } = null!;
  }
