using System;
using System.Collections.Generic;

namespace MC_GymMasterWebAPI.Models;

public partial class ShareBoard
{
    public int ShareBoardId { get; set; }

    public int MemberId { get; set; }

    public byte[]? ProfileImage { get; set; }

    public int? LikeImage { get; set; }

    public DateOnly CreationDate { get; set; }

    public DateOnly ExpirationDate { get; set; }

    public DateOnly LastModified { get; set; }

    public virtual Member Member { get; set; } = null!;

    public virtual ICollection<ShareBoardComment> ShareBoardComments { get; set; } = new List<ShareBoardComment>();
}
