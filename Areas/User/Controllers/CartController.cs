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
            .ThenInclude(c => c.Category)
            .Where(c => c.UserId == user.Id)
            .ToListAsync();

        var activeCoupons = await _context.Coupons
            .Where(c => c.IsActive).ToListAsync();
        ViewBag.AvailableCoupons = activeCoupons;
        ViewBag.CartCount = cartItems.Count();
        ViewBag.Total = cartItems.Sum(i => i.MenuItem.Price * i.Quantity);

        if (TempData.ContainsKey("AppliedCoupon"))
        {
            ViewBag.AppliedCoupon = TempData["AppliedCoupon"];
        }
        return View(cartItems);
    }

    [HttpPost]
    public async Task<IActionResult> Add(int menuItemId, int quantity = 1)
    {
        var user = await _userManager.GetUserAsync(User);

        var menuitem = await _context.MenuItems.FindAsync(menuItemId);

        if (menuitem == null)
        {
            TempData["Error"] = "This menu item is no longer available.";
            return RedirectToAction("Index", "Menu");
        }
        var existingItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == user.Id && c.MenuItemId == menuItemId);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            var cartItem = new CartItem
            {
                UserId = user.Id,
                MenuItemId = menuItemId,
                Quantity = quantity,
            };

            _context.CartItems.Add(cartItem);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int id)
    {
        var item = await _context.CartItems
            .Include(c => c.MenuItem)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (item != null)
        {
            if (item.Quantity > 1)
            {
                item.Quantity--;
            }
            else 
            {
                _context.CartItems.Remove(item);
            }
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ApplyCoupon(string couponCode)
    {
        if (string.IsNullOrWhiteSpace(couponCode))
        {
            TempData["Error"] = "Your cart is empty!";
            return RedirectToAction("Index");
        }
        var coupon = await _context.Coupons
                        .Where(c => c.Code == couponCode && c.IsActive)
                        .FirstOrDefaultAsync();

        if (coupon == null)
        {
            TempData["Error"] = "Invalid or expired coupon.";
            return RedirectToAction("Index");
        }

        TempData["AppliedCoupon"] = coupon.Code;

        TempData["Success"] = $"Coupon {coupon.Code} applied. {coupon.DiscountPercentage}% OFF!";
        return RedirectToAction(nameof(Index));
    }
}
