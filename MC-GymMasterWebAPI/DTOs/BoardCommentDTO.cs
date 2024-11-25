namespace MC_GymMasterWebAPI.DTOs
{
    public class BoardCommentDTO
    {
        public int ShareBoardId { get; set; }

        public int MemberId { get; set; }

        public string? Comment { get; set; }
    }
}
