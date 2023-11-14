using System;
using System.Collections.Generic;

namespace CarDealers.Entity;

public partial class Review
{
    public int ReviewId { get; set; }

    public int NewsId { get; set; }

    public int CustomerId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime PublishDate { get; set; }

    public bool DeleteFlag { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual News News { get; set; } = null!;
}
