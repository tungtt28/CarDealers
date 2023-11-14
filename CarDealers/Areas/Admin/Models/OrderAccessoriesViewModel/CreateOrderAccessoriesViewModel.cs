using CarDealers.Entity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarDealers.Areas.Admin.Models.OrderAccessoriesViewModel
{
	public class CreateOrderAccessoriesViewModel
	{
        public int? CustomerId { get; set; }
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? Address { get; set; }
        public int OrderId { get; set; }
		public List<CartItem> Details { get; set; } = new List<CartItem>();
		public IEnumerable<SelectListItem> OrderList { get; set; } = new List<SelectListItem>();
		public int AccessoryId { get; set; }
        public IEnumerable<SelectListItem>? AccessoryList { get; set; }
		public IEnumerable<int>? SelectedAccessories { get; set; }
		public int CouponId { get; set; }
		public IEnumerable<SelectListItem> CouponList { get; set; } = new List<SelectListItem>();
		public int? SellerId { get; set; }
		public IEnumerable<SelectListItem> SellerList { get; set; } = new List<SelectListItem>();
		public int Status { get; set; }
		public IEnumerable<SelectListItem> StatusList { get; set; } = new List<SelectListItem>
		{
            new SelectListItem { Value = "0", Text = "Đang tạo đơn" },
            new SelectListItem { Value = "1", Text = "Đang chờ thanh toán" },
			new SelectListItem { Value = "2", Text = "Đã thanh toán" },
            new SelectListItem { Value = "3", Text = "Giao hàng thành công" },
            new SelectListItem { Value = "4", Text = "Giao hàng không thành công" },
			new SelectListItem { Value = "5", Text = "Hủy đơn hàng" }
        };

		public int Quantity { get; set; }

		public decimal? TotalPrice { get; set; }
	
	}

	
}
