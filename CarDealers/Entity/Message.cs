using System;
using System.Collections.Generic;

namespace CarDealers.Entity;

public partial class Message
{
    public int MessageId { get; set; }

    public string MessageContent { get; set; } = null!;

    public int EmployeeId { get; set; }

    public int CustomerId { get; set; }

    public DateTime SendTime { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual User Employee { get; set; } = null!;
}
