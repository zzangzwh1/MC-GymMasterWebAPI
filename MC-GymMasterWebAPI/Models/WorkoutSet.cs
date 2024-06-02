namespace MC_GymMasterWebAPI.Models
{
    public class WorkoutSet
    {
        public int WorkoutSetId { get; set; }
        public int MemberId { get; set;}
        public string Part { get; set; } = "";
        public int SetCount { get; set; }
        public string SetDescription { get; set; } = "";
        public DateTime CreationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime LastModified { get; set; }


    }
}
