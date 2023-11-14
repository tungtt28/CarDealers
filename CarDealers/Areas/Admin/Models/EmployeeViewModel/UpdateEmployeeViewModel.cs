using Microsoft.AspNetCore.Mvc.Rendering;
using CarDealers.Entity;
using System.ComponentModel.DataAnnotations;

namespace CarDealers.Areas.Admin.Models.EmployeeViewModel
{
    public class UpdateEmployeeViewModel
    {
        public int EmployeeId { get; set; }

        public int UserRoleId { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public string Dob { get; set; }

        public bool Gender { get; set; }

        public int Status { get; set; }

        public string Username { get; set; } = null!;

        public string? Password { get; set; }

        public string Email { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string Address { get; set; } = null!;
    }
}
