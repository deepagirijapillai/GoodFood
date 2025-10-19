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

            var restaurants = _context.Restaurants
                .Where(r => r.ApplicationUserId == id)
                .ToList();

            if (restaurants.Any())
            {
                var restaurantIds = restaurants.Select(r => r.Id).ToList();

                var pendingOrders = _context.OrderDetails
                    .Include(o => o.Order)
                    .Include(o => o.MenuItem)
                    .Where(o => restaurantIds.Contains(o.MenuItem.RestaurantId) && o.Order.Status == "Pending")
                    .Select(o => o.Order)
                    .Distinct()
                    .ToList();

                if (pendingOrders.Any())
                {
                    TempData["Error"] = $"Cannot delete this user — {pendingOrders.Count} pending orders exist for their restaurant(s)."; ;
                    return RedirectToAction("Index");
                }
            }

            foreach (var rest in restaurants)
            {
                rest.IsActive = false;

                var menuItems = _context.MenuItems
                    .Where(m => m.RestaurantId == rest.Id)
                    .ToList();
                foreach (var item in menuItems)
                { item.IsActive = false; }

                var offers = _context.Offers
                    .Where(m => m.RestaurantOwnerId == rest.ApplicationUserId)
                    .ToList();
                foreach (var item in offers)
                { item.IsActive = false; }

            }

            _context.SaveChanges();
            transaction.Commit();

            TempData["Success"] = "User soft-deleted successfully (deactivated).";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            if (transaction != null) transaction.Rollback();
            TempData["Error"] = $"Error while deleting user: {ex.Message}";
            return RedirectToAction("Index");
        }
    }
}
