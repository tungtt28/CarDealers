using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CarDealers.Areas.Admin.Models.AccessoriesViewModel
{
    public class ListAccessoriesViewModel
    {
        public int AccessoryId { get; set; }

        public string Quantity { get; set; }

        public string AccessoryName { get; set; } = null!;

        public string? ImportPrice { get; set; }

        public string? ExportPrice { get; set; }

        public string? Description { get; set; }

        public string? Image { get; set; } = null!;

        public bool Status { get; set; }

        public bool DeleteFlag { get; set; }
    }
}
