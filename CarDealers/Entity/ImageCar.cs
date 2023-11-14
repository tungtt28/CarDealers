using System;
using System.Collections.Generic;

namespace CarDealers.Entity;

public partial class ImageCar
{
    public int ImageId { get; set; }

    public int CarId { get; set; }

    public string Image { get; set; } = null!;

    public bool DeleteFlag { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual Car Car { get; set; } = null!;
}
