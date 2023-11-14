using CarDealers.Entity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CarDealers.Models.BookingServiceModel
{
    public class CreateGuestBookingServiceViewModel
    {
        public string Email { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public int? CarId { get; set; }
        public IEnumerable<SelectListItem> CarList { get; set; } = new List<SelectListItem>();

        public string? Kilometerage { get; set; }

        public string? PlateNumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public string DateBooking { get; set; }

        public string? Note { get; set; }

        public List<ServiceType>? ServiceTypes { get; set; }

        public List<int>? SelectedServiceIds { get; set; }
    }
}
