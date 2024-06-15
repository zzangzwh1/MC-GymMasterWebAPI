namespace MC_GymMasterWebAPI.DTOs
{
    public class MemberDTO
    {
      
        public string UserId { get; set; } = null!;

        public char Sex { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public DateTime? BirthDate { get; set; }

        public string? Address { get; set; }

        public string Email { get; set; } = null!;

        public string? Phone { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime ExpirationDate { get; set; }
    }
}
