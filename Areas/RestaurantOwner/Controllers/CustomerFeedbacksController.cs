using GoodFood.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GoodFood.Areas.RestaurantOwner.Controllers;

[Area("RestaurantOwner")]
[Authorize(Roles = "RestaurantOwner")]
public class CustomerFeedbacksController : Controller
{
    private readonly ApplicationDbContext _context;

    public CustomerFeedbacksController(ApplicationDbContext context) 
    {
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var feedbacks = await _context.CustomerFeedbacks
            .Where(o=>o.RestaurantOwnerId==userId)
            .OrderByDescending(f=>f.CreatedDate)
            .ToListAsync();

        return View(feedbacks);
    }

    public async Task<IActionResult> Details(int id)
    {
        var feedback = await _context.CustomerFeedbacks.FindAsync(id);
        if(feedback == null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(feedback.RestaurantOwnerId!=userId) return Forbid();
        return View(feedback);
    }
}
