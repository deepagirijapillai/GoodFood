using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoodFood.Models;
public class MenuItem
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [Display(Name = "Dish Name")]
    public string? Name { get; set; }
    public string Description { get; set; }

    [Range(1, 10000)]
    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }

    [Required]
    public int RestaurantId { get; set; }

    [ForeignKey("RestaurantId")]
    [ValidateNever]
    public Restaurant Restaurant { get; set; }
    public int? CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    [ValidateNever]
    public Category Category { get; set; }

    public int? SubCategoryId { get; set; }

    [ForeignKey("SubCategoryId")]
    [ValidateNever]
    public SubCategory SubCategory { get; set; }
}
