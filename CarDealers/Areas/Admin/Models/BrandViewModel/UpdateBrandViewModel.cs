namespace CarDealers.Areas.Admin.Models.BrandViewModel
{
    public class UpdateBrandViewModel
    {
        public int BrandId { get; set; }

        public string BrandName { get; set; } = null!;

        public bool DeleteFlag { get; set; }

        public string? LogoImage { get; set; } = null!;
    }
}
