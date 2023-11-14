using System;
using System.Collections.Generic;

namespace CarDealers.Entity;

public partial class Car
{
    public int CarId { get; set; }

    public int BrandId { get; set; }

    public int CarTypeId { get; set; }

    public int Quantity { get; set; }

    public string Model { get; set; } = null!;

    public int? Year { get; set; }

    public decimal? ImportPrice { get; set; }

    public decimal? ExportPrice { get; set; }

    public decimal? DepositPrice { get; set; }

    public decimal? Tax { get; set; }

    public int? Mileage { get; set; }

    public int EngineTypeId { get; set; }

    public int FuelTypeId { get; set; }

    public string? Transmission { get; set; }

    public string? Description { get; set; }

    public string Image { get; set; } = null!;

    public bool Status { get; set; }

    public bool DeleteFlag { get; set; }

    public string? Content { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual Brand Brand { get; set; } = null!;

    public virtual ICollection<CarTrial> CarTrials { get; set; } = new List<CarTrial>();

    public virtual CarType CarType { get; set; } = null!;

    public virtual ICollection<ColorCarRefer> ColorCarRefers { get; set; } = new List<ColorCarRefer>();

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual EngineType EngineType { get; set; } = null!;

    public virtual FuelType FuelType { get; set; } = null!;

    public virtual ICollection<ImageCar> ImageCars { get; set; } = new List<ImageCar>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
