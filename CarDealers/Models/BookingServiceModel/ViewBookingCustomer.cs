using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarDealers.Models.BookingServiceModel
{
    public class ViewBookingCustomer
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string CarName { get; set; }
        public string Service { get; set; }
        public string DateBooking { get; set; }
        public string PlateNumber { get; set; }
        public string Status { get; set; }
        public IEnumerable<SelectListItem> StatusList { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "0", Text = "Pending" },
            new SelectListItem { Value = "1", Text = "Processing" },
            new SelectListItem { Value = "2", Text = "Completed" },
            new SelectListItem { Value = "3", Text = "Closed" }
        };
    }
}
