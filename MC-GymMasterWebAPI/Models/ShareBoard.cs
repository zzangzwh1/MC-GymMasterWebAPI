using System;
using System.Collections.Generic;

namespace MC_GymMasterWebAPI.Models;

public partial class ShareBoard
{
    public int ShareBoardId { get; set; }

    public int MemberId { get; set; }

    public byte[]? ProfileImage { get; set; }

    public DateOnly CreationDate { get; set; }

    public DateOnly ExpirationDate { get; set; }

    public DateOnly LastModified { get; set; }

    public virtual ICollection<BoardComment> BoardComments { get; set; } = new List<BoardComment>();

    public virtual ICollection<ImageLike> ImageLikes { get; set; } = new List<ImageLike>();

    public virtual Member Member { get; set; } = null!;
}
