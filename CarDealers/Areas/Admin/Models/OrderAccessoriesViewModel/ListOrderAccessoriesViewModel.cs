using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarDealers.Areas.Admin.Models.OrderAccessoriesViewModel
{
    public class ListOrderAccessoriesViewModel
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
            new SelectListItem { Value = "0", Text = "Đang tạo đơn" },
            new SelectListItem { Value = "1", Text = "Đang chờ thanh toán" },
            new SelectListItem { Value = "2", Text = "Đã thanh toán" },
            new SelectListItem { Value = "3", Text = "Giao hàng thành công" },
            new SelectListItem { Value = "4", Text = "Giao hàng không thành công" },
            new SelectListItem { Value = "5", Text = "Hủy đơn hàng" }
        };

    }
}
