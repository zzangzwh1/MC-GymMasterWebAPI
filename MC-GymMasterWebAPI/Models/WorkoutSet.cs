using System;
using System.Collections.Generic;

namespace MC_GymMasterWebAPI.Models;

public partial class WorkoutSet
{
    public int WorkoutSetId { get; set; }

    public int MemberId { get; set; }

    public string? Part { get; set; }

    public int? SetCount { get; set; }

    public int? RepCount { get; set; }

    public int? Weight { get; set; }

    public string? SetDescription { get; set; }

    public DateOnly CreationDate { get; set; }

    public DateOnly ExpirationDate { get; set; }

    public DateOnly LastModified { get; set; }

    public virtual Member Member { get; set; } = null!;
}
