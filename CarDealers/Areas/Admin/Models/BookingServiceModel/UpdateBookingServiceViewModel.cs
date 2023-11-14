using CarDealers.Entity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CarDealers.Areas.Admin.Models.BookingServiceModel
{
    public class UpdateBookingServiceViewModel
    {
        public int BookingId { get; set; }

        public int? CustomerId { get; set; }

        public string Email { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public string DateBooking { get; set; }

        public int? CarId { get; set; }
        public IEnumerable<SelectListItem> CarList { get; set; } = new List<SelectListItem>();

        public string? Kilometerage { get; set; }

        public string? PlateNumber { get; set; }

        public string? Note { get; set; }

        public List<ServiceType>? ServiceTypes { get; set; }

        public List<int>? SelectedServiceIds { get; set; }

        public int? Status { get; set; }

        public IEnumerable<SelectListItem> StatusList { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "0", Text = "Pending" },
            new SelectListItem { Value = "1", Text = "Processing" },
            new SelectListItem { Value = "2", Text = "Completed" },
            new SelectListItem { Value = "3", Text = "Closed" }
        };
    }
}
