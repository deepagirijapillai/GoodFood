using GoodFood.Data;
using GoodFood.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoodFood.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var vm = new AdminDashboardModel()
        {
            TotalUsers = await _userManager.Users.CountAsync(),
            TotalCategories = await _context.Categories.CountAsync(),
            TotalOrders = await _context.Orders.CountAsync(o=>o.IsActive),
            ActiveCoupons = await _context.Coupons.CountAsync(c=>c.IsActive),

        };
        return View(vm);
    }
}
