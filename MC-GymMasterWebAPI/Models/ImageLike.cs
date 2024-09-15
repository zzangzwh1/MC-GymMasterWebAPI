using System;
using System.Collections.Generic;

namespace MC_GymMasterWebAPI.Models;

public partial class ImageLike
{
    public int LikeImageId { get; set; }

    public int ShareBoardId { get; set; }

    public int MemberId { get; set; }

    public int ImageLike1 { get; set; }

    public DateOnly CreationgDate { get; set; }

    public DateOnly ExpirationDate { get; set; }

    public DateOnly LastModifiedDate { get; set; }

    public virtual ShareBoard ShareBoard { get; set; } = null!;
}
