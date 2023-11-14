using System;
using System.Collections.Generic;

namespace CarDealers.Entity;

public partial class Customer
{
    public int CustomerId { get; set; }

    public DateTime? Dob { get; set; }

    public bool? Gender { get; set; }

    public int? Status { get; set; }

    public int? CarId { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string Email { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? Address { get; set; }

    public int? Kilometerage { get; set; }

    public string? PlateNumber { get; set; }

    public int CustomerType { get; set; }

    public bool DeleteFlag { get; set; }
    public string? Image { get; set; } = null!;
    public bool Ads { get; set; }

    public string? VerifyCode { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();

    public virtual Car? Car { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

}
