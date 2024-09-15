using System;
using System.Collections.Generic;

namespace MC_GymMasterWebAPI.Models;

public partial class BoardComment
{
    public int BoardCommentId { get; set; }

    public int ShareBoardId { get; set; }

    public int MemberId { get; set; }

    public string? Comment { get; set; }

    public DateOnly CreationgDate { get; set; }

    public DateOnly ExpirationDate { get; set; }

    public DateOnly LastModifiedDate { get; set; }

    public virtual ShareBoard ShareBoard { get; set; } = null!;
}
