using System.ComponentModel.DataAnnotations;

namespace GoodFood.Models
{
    public class Coupon
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Coupon Code")]
        public string? Code { get; set; }

        [Required]
        [Range(1,100)]
        [Display(Name = "Discount %")]
        public int DiscountPercentage { get; set; }

        [Display(Name = "Is Active?")]
        [Required]
        public bool IsActive { get; set; }
    }
}
