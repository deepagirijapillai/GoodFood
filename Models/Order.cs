namespace GoodFood.Models;

public class Order
{
    public int Id { get; set; }

    public string? ApplicationUserId { get; set; }

    public ApplicationUser? ApplicationUser { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public double TotalAmount { get; set; }
    public string Status { get; set; } = "Pending";

    public int? CouponId { get; set; }
    public Coupon? Coupon { get; set; }

    public int? OfferId { get; set; }
    public Offer? Offer { get; set; }

    public double? DiscountApplied { get; set; }

    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
