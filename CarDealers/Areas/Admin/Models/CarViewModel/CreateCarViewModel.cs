using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CarDealers.Areas.Admin.Models.CarViewModel
{
    public class CreateCarViewModel
    {

        public int BrandId { get; set; }
        public IEnumerable<SelectListItem> BrandList { get; set; } = new List<SelectListItem>();
        public int CarTypeId { get; set; }
        public IEnumerable<SelectListItem> CarTypeList { get; set; } = new List<SelectListItem>();
        public int EngineTypeId { get; set; }
        public IEnumerable<SelectListItem> EngineTypeList { get; set; } = new List<SelectListItem>();
        public int FuelTypeId { get; set; }
        public IEnumerable<SelectListItem> FuelTypeList { get; set; } = new List<SelectListItem>();
        public int Status { get; set; }
        public IEnumerable<SelectListItem> StatusList { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "0", Text = "Active" },
            new SelectListItem { Value = "1", Text = "Inactive" }
        };

        [Range(1, 999999999999999999, ErrorMessage = "Quantity must be greater than one.")]
        public string Quantity { get; set; }
         
        public string Model { get; set; } = null!;

        [RegularExpression(@"^\d{1,18}$", ErrorMessage = "Year must be between 1 and 18 digits.")]
        [Range(1, double.MaxValue, ErrorMessage = "Year must be greater than one.")]
        public int Year { get; set; }

        [RegularExpression(@"^\d{1,16}(\.\d{1,2})?$", ErrorMessage = "ImportPrice must be a number with up to 16 digits and up to 2 decimal places.")]
        [Range(1, double.MaxValue, ErrorMessage = "ImportPrice must be greater than one.")]
        [Required(ErrorMessage = "ImportPrice is required.")]
        public decimal ImportPrice { get; set; }

        [RegularExpression(@"^\d{1,16}(\.\d{1,2})?$", ErrorMessage = "ExportPrice must be a number with up to 16 digits and up to 2 decimal places.")]
        [Range(1, double.MaxValue, ErrorMessage = "ExportPrice must be greater than one.")]
        [Required(ErrorMessage = "ExportPrice is required.")]
        public decimal ExportPrice { get; set; }

        [RegularExpression(@"^\d{1,16}(\.\d{1,2})?$", ErrorMessage = "DepositPrice must be a number with up to 16 digits and up to 2 decimal places.")]
        [Range(1, double.MaxValue, ErrorMessage = "DepositPrice must be greater than one.")]
        [Required(ErrorMessage = "DepositPrice is required.")]
        public decimal DepositPrice { get; set; }

        [RegularExpression(@"^\d{1,16}(\.\d{1,2})?$", ErrorMessage = "Tax must be a number with up to 16 digits and up to 2 decimal places.")]
        [Range(1, double.MaxValue, ErrorMessage = "Tax must be greater than one.")]
        [Required(ErrorMessage = "Tax is required.")]
        public decimal Tax { get; set; }

        public IEnumerable<SelectListItem>? Colors { get; set; }
        public IEnumerable<int>? SelectedColors { get; set; }

        [Range(0, 999999999999999999, ErrorMessage = "Milage must be greater than one.")]
        public string? Mileage { get; set; }

        public string? Transmission { get; set; }

        public string? Description { get; set; }

        public string? Image { get; set; }
        public string? Content { get; set; }
    }
}
