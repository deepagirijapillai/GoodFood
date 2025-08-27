using System.ComponentModel.DataAnnotations;

namespace GoodFood.Models;

public class Offer
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    [Range(1,100)]
    public double DiscountPercentage { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }

    public string? RestaurantOwnerId { get; set; }

    public ApplicationUser? RestaurantOwner { get; set; }

}
