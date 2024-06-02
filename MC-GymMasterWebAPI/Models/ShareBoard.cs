namespace MC_GymMasterWebAPI.Models
{
    public class ShareBoard
    {
        public int ShareBoardId { get; set; }
        public int MemberId { get; set; }
        public byte[] ProfileImage { get; set; }
        public int LikeImage { get; set; }
        public string Comment { get; set; } = "";
        public DateTime CreationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime LastModified { get; set; }

    }
    
}
