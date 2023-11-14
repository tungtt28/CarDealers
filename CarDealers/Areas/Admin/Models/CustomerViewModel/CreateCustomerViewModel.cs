using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CarDealers.Areas.Admin.Models.CustomerViewModel
{
    public class CreateCustomerViewModel
    {
        public int CustomerId { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public string Dob { get; set; }

        public string? Gender { get; set; }

        public int? Status { get; set; }

        public int? CarId { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }

        public string Email { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string? Address { get; set; }

        public int? Kilometerage { get; set; }

        public string? PlateNumber { get; set; }

        public int CustomerType { get; set; }
    }
}
