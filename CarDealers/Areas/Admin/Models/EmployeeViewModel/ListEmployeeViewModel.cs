using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using CarDealers.Entity;

namespace CarDealers.Areas.Admin.Models.EmployeeViewModel
{
    public class ListEmployeeViewModel
    {
        public int EmployeeId { get; set; }

        public string Email { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

    }
}
