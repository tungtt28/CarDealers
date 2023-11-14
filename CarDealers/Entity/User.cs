using System;
using System.Collections.Generic;

namespace CarDealers.Entity;

public partial class User
{
    public int UserId { get; set; }

    public int UserRoleId { get; set; }

    public DateTime? Dob { get; set; }

    public bool Gender { get; set; }

    public bool DeleteFlag { get; set; }

    public int Status { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Address { get; set; } = null!;

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<News> News { get; set; } = new List<News>();

    public virtual ICollection<OrderAccessoryDetail> OrderAccessoryDetails { get; set; } = new List<OrderAccessoryDetail>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual UserRole? UserRole { get; set; } = null!;
}
