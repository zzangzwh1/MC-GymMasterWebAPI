using MC_GymMasterWebAPI.DTOs;
using MC_GymMasterWebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace MC_GymMasterWebAPI.Interface
{
    public interface IGymMasterService
    {
        #region Workout
        Task<List<WorkoutSetDTO>> InsertWorkoutAsync(List<WorkoutSetDTO> insertWorkout);

        Task<List<PartCountDTO>> GetMemberWorkoutPartCountsAsync(string userId);

        Task<List<YearCountDTO>> GetAnnualWorkoutStatusAsync(string userId);

        #endregion
        #region Member
        Task<IEnumerable<Member>> GetAllMembers();
        Task<Member> GetMemberByUsername(string userId);
        Task<Member> GetMemberIdByUserId(string memberId);
        Task<MemberDTO> InsertMember(MemberDTO memberDto);
        Task<Member> Authenticate(LoginDto loginInfo);

        #endregion

    }
}
