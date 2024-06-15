using System;
using System.Collections.Generic;

namespace MC_GymMasterWebAPI.Model;

public partial class MemberConnection
{
    public int MemberConnectionId { get; set; }

    public int MemberId { get; set; }

    public string? Followings { get; set; }

    public string? Followers { get; set; }

    public DateOnly CreationDate { get; set; }

    public DateOnly ExpirationDate { get; set; }

    public virtual Member Member { get; set; } = null!;
}
