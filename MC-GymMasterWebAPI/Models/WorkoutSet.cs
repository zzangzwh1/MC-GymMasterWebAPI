using System;
using System.Collections.Generic;

namespace MC_GymMasterWebAPI.Model;

public partial class WorkoutSet
{
    public int WorkoutSetId { get; set; }

    public int MemberId { get; set; }

    public string? Part { get; set; }

    public int? SetCount { get; set; }
    public int? RepCount { get; set; }
    public int? Weight { get; set; }

    public string? SetDescription { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime ExpirationDate { get; set; }

    public DateTime LastModified { get; set; }

    public virtual Member Member { get; set; } = null!;
}
