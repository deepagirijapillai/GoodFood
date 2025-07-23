using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodFood.Areas.RestaurantOwner.Controllers;

[Area("RestaurantOwner")]
[Authorize(Roles = "RestaurantOwner")]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
