namespace MC_GymMasterWebAPI.DTOs
{
    public class MemberAndCommentInfoDTO
    {
        public string UserId { get; set; } = "";
        public string FirstName { get; set; } = "";

        public string LastName { get; set; } = "";

        public string? Address { get; set; }

        public string Email { get; set; } = "";

        public string? Phone { get; set; }
        public int ShareBoardId { get; set; }

        public int MemberId { get; set; }

        public string? Comment { get; set; }
    }
}
