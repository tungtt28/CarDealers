using System;
using System.Collections.Generic;

namespace CarDealers.Entity;

public partial class BookingRefer
{
    public int BookingId { get; set; }

    public int ServiceTypeId { get; set; }

    public virtual BookingService Booking { get; set; } = null!;

    public virtual ServiceType ServiceType { get; set; } = null!;
}
