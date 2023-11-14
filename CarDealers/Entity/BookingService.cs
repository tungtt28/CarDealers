using System;
using System.Collections.Generic;

namespace CarDealers.Entity;

public partial class BookingService
{
    public int BookingId { get; set; }

    public int? CustomerId { get; set; }
    public int? CustomerParentId { get; set; }

    public DateTime DateBooking { get; set; }

    public string? Note { get; set; }

    public int Status { get; set; }

    public bool DeleteFlag { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<ServiceType> ServiceTypes { get; set; } = new List<ServiceType>();
    public virtual ICollection<BookingRefer> BookingRefers { get; set; } = new List<BookingRefer>();
}
