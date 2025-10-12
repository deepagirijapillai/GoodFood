using System.ComponentModel.DataAnnotations;

namespace GoodFood.Models;

public class AdminDashboardModel
{
    public int TotalUsers { get; set; }
    public int TotalCategories { get; set; }
    public int TotalOrders { get; set; }
    public int ActiveCoupons { get; set; }

}