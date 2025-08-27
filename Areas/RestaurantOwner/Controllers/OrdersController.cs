using GoodFood.Data;
using GoodFood.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoodFood.Areas.RestaurantOwner.Controllers;

[Area("RestaurantOwner")]
[Authorize(Roles = "RestaurantOwner")]
public class OrdersController : Controller
{
    private readonly ApplicationDbContext _context;
    public OrdersController(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        var orders = await _context.Orders
                    .Include(o => o.ApplicationUser)
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.MenuItem)
                    .ToListAsync();
        return View(orders);
    }
    public async Task<IActionResult> Details(int id)
    {
        var order = await _context.Orders
                    .Include(o => o.ApplicationUser)
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.MenuItem)
                    .FirstOrDefaultAsync(o=>o.Id==id);
        return order==null ? NotFound() : View(order);
    }
}
