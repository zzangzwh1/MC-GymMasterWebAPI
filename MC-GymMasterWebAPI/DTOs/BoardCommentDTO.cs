﻿namespace MC_GymMasterWebAPI.DTOs
{
    public class BoardCommentDTO
    {       
        public int BoardCommentId { set; get; }
        public int ShareBoardId { get; set; }

        public string MemberId { get; set; }

        public string? Comment { get; set; }
    }
}
