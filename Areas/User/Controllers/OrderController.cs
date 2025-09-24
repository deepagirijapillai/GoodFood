using GoodFood.Data;
using GoodFood.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoodFood.Areas.User.Controllers;

[Area("User")]
[Authorize(Roles = "User")]
public class OrderController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrderController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var orders = _context.Orders
            .Include(c => c.OrderDetails)
            .ThenInclude(c => c.MenuItem)
            .Where(c => c.ApplicationUserId == user.Id)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return View(orders);
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder()
    {
        var user = await _userManager.GetUserAsync(User);

        var cartItems = await _context.CartItems
            .Include(c => c.MenuItem)
            .Where(c => c.UserId == user.Id)
            .ToListAsync();

        if (!cartItems.Any())
        {
            TempData["Error"] = "Your cart is empty!";
            return RedirectToAction("Index", "Cart");
        }

        var order = new Order
        {
            ApplicationUserId = user?.Id,
            OrderDate = DateTime.Now,
            Status = "Pending",
            TotalAmount = 0
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        foreach (var item in cartItems)
        {
            var detail = new OrderDetail
            {
                OrderId = order.Id,
                MenuItemId = item.MenuItemId,
                Quantity = item.Quantity,
                Price = (double)item.MenuItem.Price,
            };
            _context.OrderDetails.Add(detail);
            order.TotalAmount += detail.Quantity * detail.Price;
            await _context.SaveChangesAsync();
        }

        _context.CartItems.RemoveRange(cartItems);
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Order placed successfully";
        return RedirectToAction("Index");

    }
}
