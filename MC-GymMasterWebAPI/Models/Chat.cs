namespace MC_GymMasterWebAPI.Models
{
    public class Chat
    {
        public int ChatId { get; set; }
        public int MemberId { get; set; }
        public string Comment { get; set; } = "";
        public DateTime CreationDate { get; set; } 
        public DateTime ExpirationDate { get; set; }
    }
}
