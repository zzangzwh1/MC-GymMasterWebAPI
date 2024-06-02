namespace MC_GymMasterWebAPI.Models
{
    public class MemberConnection
    {
        public int MemberConnectionId { get; set; }
        public int MemberId { get; set; }
        public string Followings { get; set; } = "";
        public string Followers { get; set; } = "";
        public DateTime CreationDate { get; set; }
        public DateTime ExpirationDate { get; set; }

    }
}
