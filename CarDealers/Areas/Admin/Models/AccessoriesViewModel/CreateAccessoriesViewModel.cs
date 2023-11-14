using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CarDealers.Areas.Admin.Models.AccessoriesViewModel
{
    public class CreateAccessoriesViewModel
    {
        public int AccessoryId { get; set; }

        [Range(0, 999999999999999999, ErrorMessage = "Quantity must be greater than zero.")]
        public string Quantity { get; set; }

        public string AccessoryName { get; set; } = null!;

        [RegularExpression(@"^\d{1,16}(\.\d{1,2})?$", ErrorMessage = "ImportPrice must be a number with up to 16 digits and up to 2 decimal places.")]
        [Range(1, double.MaxValue, ErrorMessage = "ImportPrice must be greater than zero.")]
        public decimal? ImportPrice { get; set; }

        [RegularExpression(@"^\d{1,16}(\.\d{1,2})?$", ErrorMessage = "ExportPrice must be a number with up to 16 digits and up to 2 decimal places.")]
        [Range(1, double.MaxValue, ErrorMessage = "ExportPrice must be greater than zero.")]
        public decimal? ExportPrice { get; set; }

        public string? Description { get; set; }

        public string? Image { get; set; } = null!;

        public bool Status { get; set; }

        public bool DeleteFlag { get; set; }
    }
}
