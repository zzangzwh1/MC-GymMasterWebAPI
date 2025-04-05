using System;
using System.Collections.Generic;

namespace MC_GymMasterWebAPI.Models;

public partial class ImageLike
{
    public int LikeImageId { get; set; }

    public int ShareBoardId { get; set; }

    public string UserId { get; set; } = null!;

    public DateOnly CreationDate { get; set; }

    public DateOnly ExpirationDate { get; set; }

    public DateOnly LastModifiedDate { get; set; }

    public virtual ShareBoard ShareBoard { get; set; } = null!;
}
