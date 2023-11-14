namespace CarDealers.Entity;

public partial class FuelType
{
    public int FuelTypeId { get; set; }

    public string FuelTypeName { get; set; } = null!;

    public bool DeleteFlag { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}
