using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodFood.Areas.User.Controllers;

[Area("User")]
[Authorize(Roles = "User")]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
