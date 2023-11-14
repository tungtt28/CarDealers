using CarDealers.Entity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CarDealers.Areas.Admin.Models.BookingServiceModel
{
    public class ListBookingServiceViewModel
    {
        public int BookingId { get; set; }

        public string Email { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public string DateBooking { get; set; }

        public string? PlateNumber { get; set; }

        public string? Status { get; set; }
    }
}
