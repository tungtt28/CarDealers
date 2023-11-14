using System;
using System.Collections.Generic;

namespace CarDealers.Entity;

public partial class ServiceType
{
    public int ServiceTypeId { get; set; }

    public string ServiceTypeName { get; set; } = null!;

    public string? Description { get; set; }

    public bool DeleteFlag { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual ICollection<BookingService> Bookings { get; set; } = new List<BookingService>();
    public virtual ICollection<BookingRefer> BookingRefers { get; set; } = new List<BookingRefer>();
}
