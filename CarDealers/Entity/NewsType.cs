using System;
using System.Collections.Generic;

namespace CarDealers.Entity;

public partial class NewsType
{
    public int NewsTypeId { get; set; }

    public string NewsTypeName { get; set; } = null!;

    public string? Description { get; set; }

    public bool DeleteFlag { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual ICollection<News> News { get; set; } = new List<News>();
}
