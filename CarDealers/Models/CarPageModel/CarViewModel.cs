namespace CarDealers.Models.CarPageModel
{
	public class CarViewModel
	{
		public int CarId { get; set; }
		public string CarName { get; set; } = null!;
		public string? ExportPrice { get; set; }
		public string? Image { get; set; } = null!;
	}
}
