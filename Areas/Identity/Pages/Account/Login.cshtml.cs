using GoodFood.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NuGet.Protocol.Plugins;
using System.ComponentModel.DataAnnotations;

namespace GoodFood.Areas.Identity.Pages.Account;
public class LoginModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    public LoginModel(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [BindProperty]
    public InputModel? Input { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
    public async Task<IActionResult> OnPostAsync()
    {
        Console.WriteLine("on post reached");
        var returnUrl = null ?? Url.Content("~/");
        Console.WriteLine("ReturnUrl: " + returnUrl);
        if (ModelState.IsValid)
        {
            var user = await _signInManager.UserManager.FindByEmailAsync(Input.Email);
            Console.WriteLine(user == null ? "User not found" : $"User found: {user.Email}");

            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, Input.Password, false, false);
                if (result.Succeeded)
                {
                    var roles = await _signInManager.UserManager.GetRolesAsync(user);
                    if (roles.Contains("Admin"))
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    else if (roles.Contains("RestaurantOwner"))
                        return RedirectToAction("Index", "Dashboard", new { area = "RestaurantOwner" });
                    //else if (roles.Contains("DeliveryPerson"))
                    //    return RedirectToAction("Index", "Delivery", new { area = "DeliveryPerson" });
                    Console.WriteLine("Login success");

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return LocalRedirect(returnUrl);
                }
                else
                {
                    Console.WriteLine($"Login failed: {result}");
                }
            }
        }
        ModelState.AddModelError("", "Invalid Login Attempt");
        return Page();
    }

}