using MC_GymMasterWebAPI.DTOs;
using MC_GymMasterWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace MC_GymMasterWebAPI.Interface
{
    public interface IGymMasterService
    {
        #region Workout
        Task<List<WorkoutSetDTO>> InsertWorkoutAsync(List<WorkoutSetDTO> insertWorkout);
        Task<List<PartCountDTO>> GetMemberWorkoutPartCountsAsync(string userId);
        Task<List<YearCountDTO>> GetAnnualWorkoutStatusAsync(string id);

        #endregion
        #region Member
        Task<IEnumerable<Member>> GetAllMembers();       
        Task<Member> GetMemberIdByUserId(string memberId);
        Task<MemberDTO> InsertMember(MemberDTO memberDto);
        Task<Member> Authenticate(LoginDto loginInfo);
        Task<Member> UpdateUserInfo(MemberDTO member);

        #endregion

        #region Image
        Task<List<ShareBoardImages>> GetEveryMemberImage();
        Task<List<ShareBoardImages>> GetMemberImage(int memberId);
        Task UploadImage(IFormFile image, int memberId);
        Task<List<ImageLikeDTO>> GetLikedImage(string member);
        Task<string> UploadImageLike(ImageLikeDTO like);
        Task<ShareBoard> DeleteImage(int shareBoardId);
        Task<IList<ShareBoardImages>> GetScrollDownCurrentPageImages(int shardboardId, int page);
        Task<IList<ShareBoardImages>> GetScrollUpCurrentPageImages(int shardboardId, int page);

        Task<List<ImageLikeCountDTO>> GetLikedImage();

        #endregion

        #region BoardComment
        Task<BoardComment> AddComment(BoardCommentDTO comments);
        Task<IList<MemberAndCommentInfoDTO>> GetComments(IList<ShareBoardImages> images);
        Task<Result> EditComment(int boardCommendId, BoardCommentDTO comment);
        Task<Result> DeleteComment(int boardCommendId);
        #endregion

        #region Email
        Task<bool> SendMail(MailData Mail_Data);

        #endregion
    }
}
