using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoodFood.Models;
public class Restaurant
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Restaurant Name")]
    public string? Name { get; set; }

    [Required]
    public string Address { get; set; }
    public string Description { get; set; }

    [ValidateNever]
    public string ApplicationUserId { get; set; }

    [ValidateNever]
    [ForeignKey("ApplicationUserId")]
    public ApplicationUser ApplicationUser { get; set; }

}