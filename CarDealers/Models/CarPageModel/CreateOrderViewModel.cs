namespace CarDealers.Models.CarPageModel
{
    public class CreateOrderViewModel
	{
		public int CarId { get; set; }
		public string FullName { get; set; } = null!;
		public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Quantity { get; set; }
		public string? Price { get; set; }
        public string? DepositePrice { get; set; }
		public DateTime? CreatedOn { get; set; }
	}


}
