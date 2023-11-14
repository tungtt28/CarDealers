using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CarDealers.Areas.Admin.Models.OrderCarViewModel
{
    public class CreateOrderCarCustomerViewModel
    {
        public int OrderId { get; set; }
        public IEnumerable<SelectListItem> OrderList { get; set; } = new List<SelectListItem>();
        public int CarId { get; set; }
        public IEnumerable<SelectListItem> CarList { get; set; } = new List<SelectListItem>();
        public int ColorId { get; set; }
        public IEnumerable<SelectListItem> ColorList { get; set; } = new List<SelectListItem>();
        public int? CouponId { get; set; }
        public IEnumerable<SelectListItem> CouponList { get; set; } = new List<SelectListItem>();
        public string? SellerName { get; set; }
        public string? CustomerName { get; set; }
        public int? SellerId { get; set; }
        public IEnumerable<SelectListItem> SellerList { get; set; } = new List<SelectListItem>();
        public int CustomerId { get; set; }
        public IEnumerable<SelectListItem> CustomerList { get; set; } = new List<SelectListItem>();
        public int Status { get; set; }
        public IEnumerable<SelectListItem> StatusList { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "0", Text = "Creating an order" },
            new SelectListItem { Value = "1", Text = "Awaiting deposit" },
            new SelectListItem { Value = "2", Text = "Deposited 15%" },
            new SelectListItem { Value = "3", Text = "Wait for pay" },
            new SelectListItem { Value = "4", Text = "Paid" },
            new SelectListItem { Value = "5", Text = "Complete the order" }
        };
        [RegularExpression(@"^\d{1,16}(\.\d{1,2})?$", ErrorMessage = "Quantity must be a number with up to 16 digits and up to 2 decimal places.")]
        [Range(1, double.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public int Quantity { get; set; }
        public decimal? TotalPrice { get; set; }
    }

}
