using System;
using System.Collections.Generic;

namespace CarDealers.Entity;

public partial class AutoAccessory
{
    public int AccessoryId { get; set; }

    public int Quantity { get; set; }

    public string AccessoryName { get; set; } = null!;

    public decimal? ImportPrice { get; set; }

    public decimal? ExportPrice { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; } = null!;

    public bool Status { get; set; }

    public bool DeleteFlag { get; set; }
    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual ICollection<OrderAccessoryDetail> OrderAccessoryDetails { get; set; } = new List<OrderAccessoryDetail>();
}
