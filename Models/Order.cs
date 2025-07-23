using System.ComponentModel.DataAnnotations;

namespace GoodFood.Models;

public class Order
{
    public int Id { get; set; }

    public string ApplicationUserId { get; set; }

    public ApplicationUser ApplicationUser { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public double TotalAmount { get; set; }
    public string Status { get; set; } = "Pending";

    public List<OrderDetail> OrderDetail;
}
