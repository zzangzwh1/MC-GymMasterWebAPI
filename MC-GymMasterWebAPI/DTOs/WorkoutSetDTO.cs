namespace MC_GymMasterWebAPI.DTOs
{
    public class WorkoutSetDTO
    {   

        public int MemberId { get; set; }

        public string Part { get; set; } = string.Empty;

        public int? SetCount { get; set; }
        public int? RepCount { get; set; }

        public string? SetDescription { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime ExpirationDate { get; set; }

        public DateTime LastModified { get; set; }
    }
}
