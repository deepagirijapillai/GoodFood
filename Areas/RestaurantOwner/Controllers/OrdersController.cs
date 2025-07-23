using GoodFood.Data;
using GoodFood.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoodFood.Areas.RestaurantOwner.Controllers;

public class OrdersController : Controller
{
    private readonly ApplicationDbContext _context;
    public OrdersController(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        var orders = await _context.Orders.Include(o => o.ApplicationUser)
                    .Include(od => od.OrderDetail)
                    .ThenInclude(od => od.MenuItem)
                    .ToListAsync();
        return View(orders);
    }
}
