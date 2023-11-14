using System;
using System.Collections.Generic;

namespace CarDealers.Entity;

public partial class UserRole
{
    public int UserRoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public bool DeleteFlag { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
