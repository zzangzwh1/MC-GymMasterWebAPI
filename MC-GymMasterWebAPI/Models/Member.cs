namespace MC_GymMasterWebAPI.Models
{
    public class Member
    {
        public int MemberId { get; set; }
        public string UserID { get; set; } = "";
        public char Sex { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public DateTime BirthDate { get; set; }
        public string Address { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public DateTime CreationDate { get; set; }
        public DateTime ExpirationDate { get; set; }

    }
}
