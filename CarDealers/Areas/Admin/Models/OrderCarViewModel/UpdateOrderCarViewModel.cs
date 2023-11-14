using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CarDealers.Areas.Admin.Models.OrderCarViewModel
{
    public class UpdateOrderCarViewModel
    {
        public int OrderId { get; set; }
        public int OrderDetailId { get; set; }
        public string CarModel { get; set; }
        public string Color { get; set; }
        public string Coupon { get; set; }
        public string? SellerName { get; set; }
        public int? SellerId { get; set; }
        public IEnumerable<SelectListItem> SellerList { get; set; } = new List<SelectListItem>();
        public string? CustomerName { get; set; }
        public int? Status { get; set; }
        public IEnumerable<SelectListItem> StatusList { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "0", Text = "Creating an order" },
            new SelectListItem { Value = "1", Text = "Awaiting deposit" },
            new SelectListItem { Value = "2", Text = "Deposited 15%" },
            new SelectListItem { Value = "3", Text = "Wait for pay" },
            new SelectListItem { Value = "4", Text = "Paid" },
            new SelectListItem { Value = "5", Text = "Complete the order" }
        };
        public int Quantity { get; set; }
        public decimal? TotalPrice { get; set; }
    }
}
