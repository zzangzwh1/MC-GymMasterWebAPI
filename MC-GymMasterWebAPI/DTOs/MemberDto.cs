namespace MC_GymMasterWebAPI.DTOs
{
    public class MemberDTO
    {
  
        public string UserId { get; set; } = null!;
        public string Password { get; set; } = null;

        public string Sex { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public DateOnly? BirthDate { get; set; }

        public string? Address { get; set; }

        public string Email { get; set; } = null!;

        public string? Phone { get; set; }

        public DateOnly CreationDate { get; set; }

        public DateOnly ExpirationDate { get; set; }
        public DateOnly LastModifiedDate { get; set; }


    }
}
