using System;
using System.Collections.Generic;

namespace MC_GymMasterWebAPI.Models;

public partial class ShareBoardComment
{
    public int ShareBoardCommentId { get; set; }

    public int ShareBoardId { get; set; }

    public string? Comment { get; set; }

    public DateOnly ExpirationDate { get; set; }

    public DateOnly LastModified { get; set; }

    public virtual ShareBoard ShareBoard { get; set; } = null!;
}
