using System;
using System.Collections.Generic;

namespace CarDealers.Entity;

public partial class Color
{
    public int ColorId { get; set; }

    public string ColorName { get; set; } = null!;

    public bool DeleteFlag { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual ICollection<ColorCarRefer> ColorCarRefers { get; set; } = new List<ColorCarRefer>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
