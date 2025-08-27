using GoodFood.Data;
using GoodFood.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoodFood.Areas.User.Controllers;

[Area("User")]
[Authorize(Roles = "User")]
public class CartController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var cartItems = await _context.CartItems
            .Include(c => c.MenuItem)
            .Where(c => c.UserId == user.Id)
            .ToListAsync();
        return View(cartItems);
    }

    [HttpPost]
    public async Task<IActionResult> Add(int menuItemId, int quantity = 1)
    {
        var user = await _userManager.GetUserAsync(User);

        var existingItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == user.Id && c.MenuItemId == menuItemId);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            var menuItem = await _context.MenuItems.FindAsync(menuItemId);
            if (menuItem == null) { return NotFound(); }
            var cartItem = new CartItem
            {
                UserId = user.Id,
                MenuItemId = menuItemId,
                Quantity = quantity,
            };

            _context.CartItems.Add(cartItem);
        }

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int id)
    {
        var item = await _context.CartItems.FindAsync(id);
        if (item != null) 
        { 
            _context.CartItems.Remove(item); 
            await _context.SaveChangesAsync(); 
        }

        return RedirectToAction("Index");
    }
}
