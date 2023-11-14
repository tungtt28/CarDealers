using System;
using System.Collections.Generic;

namespace CarDealers.Entity;

public partial class Coupon
{
    public int CouponId { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime DateStart { get; set; }

    public DateTime DateEnd { get; set; }

    public int UsesTotal { get; set; }

    public DateTime DateAdded { get; set; }

    public double PercentDiscount { get; set; }

    public bool Status { get; set; }

    public bool DeleteFlag { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual ICollection<OrderAccessoryDetail> OrderAccessoryDetails { get; set; } = new List<OrderAccessoryDetail>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
