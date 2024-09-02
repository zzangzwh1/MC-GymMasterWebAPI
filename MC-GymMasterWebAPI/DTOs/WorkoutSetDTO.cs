namespace MC_GymMasterWebAPI.DTOs
{
    public class WorkoutSetDTO
    {

        public int MemberId { get; set; }

        public string Part { get; set; } = string.Empty;

        public int? SetCount { get; set; }
        public int? RepCount { get; set; }
        public int? Weight { get; set; }

        public string? SetDescription { get; set; }

        public DateOnly CreationDate { get; set; }

        public DateOnly ExpirationDate { get; set; }

        public DateOnly LastModified { get; set; }
    }
}
