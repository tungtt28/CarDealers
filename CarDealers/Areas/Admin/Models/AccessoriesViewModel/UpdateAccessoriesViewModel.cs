using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CarDealers.Areas.Admin.Models.AccessoriesViewModel
{
    public class UpdateAccessoriesViewModel
    {
        public int AccessoryId { get; set; }

        [Range(0, 999999999999999999, ErrorMessage = "Quantity must be greater than zero and below 18 digits.")]
        public string Quantity { get; set; }

        public string AccessoryName { get; set; } = null!;

        [Range(0, 999999999999999999, ErrorMessage = "ImportPrice must be greater than zero and below 18 digits.")]
        public string? ImportPrice { get; set; }

        [Range(0, 999999999999999999, ErrorMessage = "ExportPrice must be greater than zero and below 18 digits.")]
        public string? ExportPrice { get; set; }
        

        public string? Description { get; set; }

        public string? Image { get; set; } = null!;

        public bool Status { get; set; }

        public bool DeleteFlag { get; set; }
    }
}
