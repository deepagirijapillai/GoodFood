using GoodFood.Data;
using GoodFood.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;

namespace GoodFood.Areas.Admin.Controllers;

[Area("Admin")]
public class UserController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public UserController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var users = _userManager.Users.ToList();
        Console.WriteLine("Total users found: " + users.Count);
        var userListWithRoles = new List<(ApplicationUser User, IList<string> Roles)>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userListWithRoles.Add((user, roles));
        }
        return View(userListWithRoles);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        using var transaction = _context.Database.BeginTransaction();
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index");
            }

            var cartItems = _context.CartItems
                .Where(c => c.UserId == id)
                .ToList();
            _context.CartItems.RemoveRange(cartItems);

            var orders = _context.Orders
                .Where(o => o.ApplicationUserId == id)
                .ToList();

            foreach (var order in orders)
            {
                var orderDetails = _context.OrderDetails
                    .Where(od => od.OrderId == order.Id)
                    .ToList();
                _context.OrderDetails.RemoveRange(orderDetails);
            }
            _context.Orders.RemoveRange(orders);

            var restaurants = _context.Restaurants
                .Where(r => r.ApplicationUserId == id)
                .ToList();

            foreach (var restarant in restaurants)
            {
                var menuItems = _context.MenuItems
                    .Where(m => m.RestaurantId == restarant.Id)
                    .ToList();
                _context.MenuItems.RemoveRange(menuItems);

                var offers = _context.Offers
                    .Where(m => m.RestaurantOwnerId == restarant.ApplicationUserId)
                    .ToList();

                var offerIds = offers.Select(o=>o.Id).ToList();
                var relatedOrders = _context.Orders
                    .Where(o => o.OfferId != null && offerIds.Contains(o.OfferId.Value))
                    .ToList();

                foreach (var order in relatedOrders)
                {
                    order.OfferId = null;
                }
                _context.Offers.RemoveRange(offers);
            }
            _context.Restaurants.RemoveRange(restaurants);

            _context.SaveChanges();

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                transaction.Rollback();
                TempData["Error"] = $"Failed to delete user.";
                return RedirectToAction("Index");
            }
            transaction.Commit();

            TempData["Success"] = "User and related data deleted successfully.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            var inner = ex.InnerException?.Message;
            Console.WriteLine($"Error: {inner}");
            TempData["Error"] = $"DB Update Error: {inner}";
            if (transaction != null) transaction.Rollback();
            TempData["Error"] = $"Error while deleting user: {ex.Message}";
            return RedirectToAction("Index");
        }
    }
}
