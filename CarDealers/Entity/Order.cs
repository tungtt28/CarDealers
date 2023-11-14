using System;
using System.Collections.Generic;

namespace CarDealers.Entity;

public partial class Order
{
    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public DateTime? OrderDate { get; set; }

    public int? Status { get; set; }

    public bool DeleteFlag { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<OrderAccessoryDetail> OrderAccessoryDetails { get; set; } = new List<OrderAccessoryDetail>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
