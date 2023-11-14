using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CarDealers.Areas.Admin.Models.CustomerViewModel
{
    public class ListCustomerViewModel
    {
        public int CustomerId { get; set; }

        public string Email { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;
    }
}
