using System;
using System.Collections.Generic;

namespace CarDealers.Entity;

public partial class EngineType
{
    public int EngineTypeId { get; set; }

    public string EngineTypeName { get; set; } = null!;

	public string? Description { get; set; }

	public bool DeleteFlag { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}
