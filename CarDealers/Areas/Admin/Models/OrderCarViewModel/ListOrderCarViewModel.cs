using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarDealers.Areas.Admin.Models.OrderCarViewModel
{
    public class ListOrderCarViewModel
    {
        public string Email { get; set; } = null!;
        public int OrderId { get; set; }
        public string? Coupon { get; set; }
        public string? SellerName { get; set; }
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Status { get; set; }
        public IEnumerable<SelectListItem> StatusList { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "0", Text = "Creating an order" },
            new SelectListItem { Value = "1", Text = "Awaiting deposit" },
            new SelectListItem { Value = "2", Text = "Deposited 15%" },
            new SelectListItem { Value = "3", Text = "Wait for pay" },
            new SelectListItem { Value = "4", Text = "Paid" },
            new SelectListItem { Value = "5", Text = "Complete the order" }
        };
    }
}
