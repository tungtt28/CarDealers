using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarDealers.Models
{
    public class OrderDetialsViewModel
    {
        public int OrderId { get; set; }

        public string ProductName { get; set; }

        public string SellerName { get; set; }

        public int Quantity { get; set; }

        public decimal? TotalPrice { get; set; }


    }
}
