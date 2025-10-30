using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Nezam.Refahi.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        ViewBag.PageTitle = "داشبورد";
        ViewBag.PTitle = "پنل مدیریت";
        return View();
    }
}