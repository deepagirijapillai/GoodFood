using System.ComponentModel.DataAnnotations;

namespace GoodFood.Models;

public class CartItem
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; }
    public ApplicationUser? User { get; set; }

    [Required]
    public int MenuItemId { get; set; }
    public MenuItem MenuItem { get; set; }

    [Range(1,20)]
    public int Quantity { get; set; }
}