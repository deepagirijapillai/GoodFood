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
        private readonly UserManager<ApplicationUser> _userManager;

        public MenuController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var menuItems = await _context.MenuItems
                .Include(c => c.Category)
                .Include(c => c.SubCategory)
                .ToListAsync();

            return View(menuItems);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int menuItemId)
        {
            var user = await _userManager.GetUserAsync(User);

            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == user.Id && c.MenuItemId == menuItemId);
            if (existingItem != null)
            {
                existingItem.Quantity += 1;
                _context.CartItems.Update(existingItem);
            }
            else
            {
                var cartItem = new CartItem
                {
                    UserId = user.Id,
                    MenuItemId = menuItemId,
                    Quantity = 1,
                };

                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
