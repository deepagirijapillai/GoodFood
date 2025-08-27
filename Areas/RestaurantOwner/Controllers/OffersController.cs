using GoodFood.Data;
using GoodFood.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GoodFood.Areas.RestaurantOwner.Controllers;

[Area("RestaurantOwner")]
[Authorize(Roles = "RestaurantOwner")]
public class OffersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public OffersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var offers = await _context.Offers
            .Include(o => o.RestaurantOwner)
            .ToListAsync();

        return View(offers);
    }

    public IActionResult Create()
    {
        return View(new Offer()
        {
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(7),
            IsActive = true
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Offer offer)
    {
        if (ModelState.IsValid)
        {
            var userId = _userManager.GetUserId(User);
            offer.RestaurantOwnerId = userId;
            _context.Offers.Add(offer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(offer);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var offer = await _context.Offers.FindAsync(id);
        return offer == null ? NotFound() : View(offer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Offer offer)
    {
        if (ModelState.IsValid)
        {
            var userId = _userManager.GetUserId(User);
            offer.RestaurantOwnerId = userId;
            _context.Offers.Update(offer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(offer);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var offer = await _context.Offers.FindAsync(id);
        if (offer != null)
        {
            _context.Offers.Remove(offer);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
