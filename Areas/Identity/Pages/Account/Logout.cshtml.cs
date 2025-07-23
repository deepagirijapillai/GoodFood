﻿using GoodFood.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GoodFood.Areas.Identity.Pages.Account;

public class LogoutModel : PageModel 
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public LogoutModel(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }
    public async Task<IActionResult> OnPost(string? returnUrl=null)
    { 
        await _signInManager.SignOutAsync();
        if (!string.IsNullOrEmpty(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }
        else
        {
            return RedirectToPage("/Account/Login");
        }
    }
}
