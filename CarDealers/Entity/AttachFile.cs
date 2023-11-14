using System;
using System.Collections.Generic;

namespace CarDealers.Entity;

public partial class AttachFile
{
    public int FileId { get; set; }

    public int OrderDetailId { get; set; }

    public string FileName { get; set; } = null!;

    public string Path { get; set; } = null!;

    public bool DeleteFlag { get; set; }

    public virtual OrderDetail OrderDetail { get; set; } = null!;

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }
}
