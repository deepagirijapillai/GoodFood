using Microsoft.AspNetCore.Identity;

namespace GoodFood.Models;
public class ApplicationUser : IdentityUser
{
    // Add custom fields here later if needed
    public bool IsActive { get; set; } = true;
}

