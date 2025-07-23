using GoodFood.Data;
using GoodFood.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GoodFood.Areas.RestaurantOwner.Controllers;

[Area("RestaurantOwner")]
[Authorize(Roles = "RestaurantOwner")]
public class MenusController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public MenusController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var menuItems = await _context.MenuItems.Include(m=>m.Restaurant).Where(r => r.Restaurant.ApplicationUserId == userId).ToListAsync();
        return View(menuItems);
    }

    public IActionResult Create()
    {
        var userId = _userManager.GetUserId(User);
        ViewBag.RestaurantList = _context.Restaurants.Where(r=>r.ApplicationUserId==userId).Select(r=>new SelectListItem 
        {
            Text = r.Name,
            Value = r.Id.ToString()
        }).ToList();
        ViewBag.CategoryList = _context.Categories.Select(r => new SelectListItem
        {
            Text = r.Name,
            Value = r.Id.ToString()
        }).ToList();
        ViewBag.SubCategoryList = _context.SubCategories.Select(r => new SelectListItem
        {
            Text = r.Name,
            Value = r.Id.ToString()
        }).ToList();
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MenuItem menuItem)
    {
        if (ModelState.IsValid)
        {
            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        var userId = _userManager.GetUserId(User);
        ViewBag.RestaurantList = _context.Restaurants.Where(r => r.ApplicationUserId == userId).Select(r => new SelectListItem
        {
            Text = r.Name,
            Value = r.Id.ToString()
        }).ToList();
        ViewBag.CategoryList = _context.Categories.Select(r => new SelectListItem
        {
            Text = r.Name,
            Value = r.Id.ToString()
        }).ToList();
        ViewBag.SubCategoryList = _context.SubCategories.Select(r => new SelectListItem
        {
            Text = r.Name,
            Value = r.Id.ToString()
        }).ToList();
        return View(menuItem);
    }
    public async Task<IActionResult> Edit(int id)
    {
        var menuItem = await _context.MenuItems.FindAsync(id);
        if (menuItem == null) return NotFound();
        var userId = _userManager.GetUserId(User);
        ViewBag.RestaurantList = _context.Restaurants.Where(r => r.ApplicationUserId == userId).Select(r => new SelectListItem
        {
            Text = r.Name,
            Value = r.Id.ToString()
        }).ToList();
        ViewBag.CategoryList = _context.Categories.Select(r => new SelectListItem
        {
            Text = r.Name,
            Value = r.Id.ToString()
        }).ToList();
        ViewBag.SubCategoryList = _context.SubCategories.Select(r => new SelectListItem
        {
            Text = r.Name,
            Value = r.Id.ToString()
        }).ToList();
        return View(menuItem);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(MenuItem menuItem)
    {
        if (ModelState.IsValid)
        {
            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        var userId = _userManager.GetUserId(User);
        ViewBag.RestaurantList = _context.Restaurants.Where(r => r.ApplicationUserId == userId).Select(r => new SelectListItem
        {
            Text = r.Name,
            Value = r.Id.ToString()
        }).ToList();
        ViewBag.CategoryList = _context.Categories.Select(r => new SelectListItem
        {
            Text = r.Name,
            Value = r.Id.ToString()
        }).ToList();
        ViewBag.SubCategoryList = _context.SubCategories.Select(r => new SelectListItem
        {
            Text = r.Name,
            Value = r.Id.ToString()
        }).ToList();
        return View(menuItem);
    }
    public async Task<IActionResult> Delete(int id)
    {
        var menuItems = await _context.MenuItems.FindAsync(id);
        if (menuItems == null) return NotFound();

        _context.MenuItems.Remove(menuItems);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
