using System;
using System.Collections.Generic;

namespace MC_GymMasterWebAPI.Model;

public partial class Chat
{
    public int ChatId { get; set; }

    public int MemberId { get; set; }

    public string Comment { get; set; } = null!;

    public DateOnly CreationDate { get; set; }

    public DateOnly ExpirationDate { get; set; }

    public virtual Member Member { get; set; } = null!;
}
