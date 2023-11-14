using CarDealers.Entity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarDealers.Areas.Admin.Models.OrderCarViewModel
{
	public class ViewOrderCarViewModel
	{
		public int OrderId { get; set; }
		public string FullName { get; set; } = null!;
		public string PhoneNumber { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string Quantity { get; set; }
		public string? Price { get; set; }
        public string? Discount { get; set; }
        public string? Deposite { get; set; }
        public string? TotalPrice { get; set; }
		public DateTime? CreatedOn { get; set; }
        public int? Status { get; set; }
    }
}
