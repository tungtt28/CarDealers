using System;
using System.Collections.Generic;

namespace CarDealers.Entity;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public int OrderId { get; set; }

    public int CarId { get; set; }

    public int ColorId { get; set; }

    public int? CouponId { get; set; }

    public int? SellerId { get; set; }

    public int Quantity { get; set; }

    public decimal? TotalPrice { get; set; }

    public bool DeleteFlag { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual ICollection<AttachFile> AttachFiles { get; set; } = new List<AttachFile>();

    public virtual Car Car { get; set; } = null!;

    public virtual Color Color { get; set; } = null!;

    public virtual Coupon? Coupon { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual User? Seller { get; set; } = null!;
}
