using System;
using System.Collections.Generic;

namespace CarDealers.Entity;

public partial class CarTrial
{
    public int CarTrialId { get; set; }

    public int CarId { get; set; }

    public int Status { get; set; }
    public string PlateNumber { get; set; }

    public bool DeleteFlag { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual Car Car { get; set; } = null!;

    public virtual ICollection<TrialDriving> TrialDrivings { get; set; } = new List<TrialDriving>();
}
