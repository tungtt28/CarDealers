using System;
using System.Collections.Generic;

namespace CarDealers.Entity;

public partial class ColorCarRefer
{
    public int ColorId { get; set; }

    public int CarId { get; set; }

    public string? Image { get; set; }

    public virtual Car Car { get; set; } = null!;

    public virtual Color Color { get; set; } = null!;
}
