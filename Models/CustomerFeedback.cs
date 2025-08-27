using System.ComponentModel.DataAnnotations;

namespace GoodFood.Models;

public class CustomerFeedback
{
    public int Id { get; set; }
        
    [Required]
    public string? CustomerName { get; set; }

    [Range(1,5)]
    public int Rating { get; set; }

    [MaxLength(100)]
    public string? Comment { get; set; }

    public DateTime CreatedDate { get; set; }  = DateTime.Now;

    public string? RestaurantOwnerId { get; set; }
    public ApplicationUser? RestaurantOwner { get; set; }


}
