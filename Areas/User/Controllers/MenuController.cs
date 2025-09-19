using GoodFood.Data;
using GoodFood.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoodFood.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User")]
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MenuController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var menuItems = await _context.MenuItems
                .Include(c => c.Category)
                .Include(c => c.SubCategory)
                .ToListAsync();

            return View(menuItems);
        }
    }
}
