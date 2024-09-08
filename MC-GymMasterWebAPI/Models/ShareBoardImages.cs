namespace MC_GymMasterWebAPI.Models
{
    public class ShareBoardImages
    {
        public int ShareBoardId { get; set; }
        public int MemberId { get; set; }
        public string? ProfileImage { get; set; } // Base64 encoded image data
        public int? LikeImage { get; set; }
        public DateOnly CreationDate { get; set; }
        public DateOnly ExpirationDate { get; set; }
        public DateOnly LastModified { get; set; }
    }
}
