using GoodFood.Data;
using GoodFood.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GoodFood.Areas.RestaurantOwner.Controllers;

[Area("RestaurantOwner")]
[Authorize(Roles = "RestaurantOwner")]
public class RestaurantsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public RestaurantsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var restaurants = await _context.Restaurants.Where(r => r.ApplicationUserId == userId).ToListAsync();

        return View(restaurants);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Restaurant restaurant)
    {
        if (ModelState.IsValid)
        {
            restaurant.ApplicationUserId = _userManager.GetUserId(User);
            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        else
        {
            foreach (var error in ModelState)
            {
                foreach (var subError in error.Value.Errors)
                {
                    Console.WriteLine($"Key : {error.Key}, Error : {error.Value.Errors}");
                }
            }
        }
        return View(restaurant);
    }
    public async Task<IActionResult> Edit(int id)
    {
        var restaurant = await _context.Restaurants.FindAsync(id);
        return restaurant == null ? NotFound() : View(restaurant);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Restaurant restaurant)
    {
        if (ModelState.IsValid)
        {
            restaurant.ApplicationUserId = _userManager.GetUserId(User);
            _context.Update(restaurant);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(restaurant);
    }
    public async Task<IActionResult> Delete(int id)
    {
        var restaurant = await _context.Restaurants.FindAsync(id);
        if (restaurant == null) return NotFound();

        _context.Restaurants.Remove(restaurant);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
