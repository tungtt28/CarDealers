using System;
using System.Collections.Generic;

namespace CarDealers.Entity;

public partial class News
{
    internal object User;

    public int NewsId { get; set; }

    public int AuthorId { get; set; }

    public int NewsTypeId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public DateTime? PublishDate { get; set; }

    public string? Image { get; set; } = null!;

    public bool DeleteFlag { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public int? Order { get; set; }

    public int? ParentId { get; set; }

    public DateTime? CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual User Author { get; set; } = null!;

    public virtual NewsType NewsType { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
