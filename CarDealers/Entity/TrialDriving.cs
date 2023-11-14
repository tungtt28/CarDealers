using System;
using System.Collections.Generic;

namespace CarDealers.Entity;

public partial class TrialDriving
{
    public int TrialId { get; set; }

    public string Email { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public int CarTrailId { get; set; }

    public DateTime DateBooking { get; set; }

    public string DriverLicense { get; set; } = null!;

    public string? Request { get; set; }

    public int Status { get; set; }

    public bool DeleteFlag { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual CarTrial CarTrail { get; set; } = null!;
}
