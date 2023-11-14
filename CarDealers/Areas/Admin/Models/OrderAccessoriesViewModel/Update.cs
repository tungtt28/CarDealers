namespace CarDealers.Areas.Admin.Models.OrderAccessoriesViewModel
{
    public class Update
    {
        public int OrderAccessoryId { get; set; }

        public int OrderId { get; set; }

        public int AccessoryId { get; set; }

        public int? CouponId { get; set; }

        public int? Quantity { get; set; }

        public decimal? TotalPrice { get; set; }

        public DateTime? CreatedOn { get; set; }

    }
}
