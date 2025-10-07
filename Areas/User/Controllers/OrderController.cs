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
    public async Task<IActionResult> PlaceOrder(string? couponCode)
    {
        var user = await _userManager.GetUserAsync(User);

        var cartItems = await _context.CartItems
            .Include(c => c.MenuItem)
            .ThenInclude(c=>c.Restaurant)
            .Where(c => c.UserId == user.Id)
            .ToListAsync();

        if (!cartItems.Any())
        {
            TempData["Error"] = "Your cart is empty!";
            return RedirectToAction("Index", "Cart");
        }
        double totalBeforeDiscount = cartItems.Sum(c => (double)c.MenuItem.Price * c.Quantity);
        double discountAppllied = 0;
        int? appliedOfferId = null; 
        int? appliedCouponId = null;

        foreach (var item in cartItems)
        {
            var offer = await _context.Offers
                .Where(c => c.RestaurantOwnerId == item.MenuItem.Restaurant.ApplicationUserId
                && c.IsActive
                && c.StartDate <= DateTime.UtcNow
                && c.EndDate >= DateTime.UtcNow)
                .FirstOrDefaultAsync();

            if (offer != null)
            {
                var offerDiscount = ((double)item.MenuItem.Price * item.Quantity) * (offer.DiscountPercentage / 100.0);
                discountAppllied += offerDiscount;
                appliedOfferId = offer.Id;
            }
            
        }

        if (!string.IsNullOrEmpty(couponCode))
        {
            var coupon = await _context.Coupons
                            .Where(c => c.Code == couponCode && c.IsActive)
                            .FirstOrDefaultAsync();

            if(coupon!=null)
            {
                var couponDiscount = totalBeforeDiscount * (coupon.DiscountPercentage / 100);
                discountAppllied += couponDiscount;
                appliedCouponId = coupon.Id;
            }
        }
        
        
        
        var order = new Order
        {
            ApplicationUserId = user?.Id,
            OrderDate = DateTime.Now,
            Status = "Pending",
            TotalAmount = totalBeforeDiscount-discountAppllied,
            DiscountApplied = discountAppllied,
            OfferId = appliedOfferId,
            CouponId = appliedCouponId
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
        }

        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Order placed successfully.Total amount after discount:{order.TotalAmount}";
        return RedirectToAction("Index");

    }
}
