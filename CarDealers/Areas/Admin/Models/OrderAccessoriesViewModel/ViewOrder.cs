using CarDealers.Entity;

namespace CarDealers.Areas.Admin.Models.OrderAccessoriesViewModel
{
	public class ViewOrder
	{
		public int OrderId { get; set; }
		public string FullName { get; set; } = null!;
		public string PhoneNumber { get; set; } = null!;
		public decimal totalPrice { get; set; }
		
		public DateTime? CreatedOn { get; set; }
		public  List<OrderAccessoryDetail> OrderAccessories { get; set; } = new List<OrderAccessoryDetail>();
	}

	
}
