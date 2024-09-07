using MC_GymMasterWebAPI.Models;

namespace MC_GymMasterWebAPI.DTOs
{
    public class ShareBoardDTO
    {     

        public int MemberId { get; set; }

        public byte[]? ProfileImage { get; set; }

        public int? LikeImage { get; set; }

        public DateOnly CreationDate { get; set; }

        public DateOnly ExpirationDate { get; set; }

        public DateOnly LastModified { get; set; }

    
    }
}
