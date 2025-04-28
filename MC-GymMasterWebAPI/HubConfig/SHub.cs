using MC_GymMasterWebAPI.DTOs;
using MC_GymMasterWebAPI.Models;
using Microsoft.AspNetCore.SignalR;
namespace MC_GymMasterWebAPI.HubConfig
{
    public class SHub :Hub
    {
        public async Task SendLikeCountUpdate(IList<ImageLikeCountDTO> imageCounts)
        {
           
            await Clients.All.SendAsync("ReceiveLikeCountUpdate", imageCounts);
        }
    }
    public class CommentHub : Hub {
        public async Task SendAddComment(IList<MemberAndCommentInfoDTO> comments)
        {
           
            await Clients.All.SendAsync("ReceiveComment", comments);
        }
    }
    public class ImageHub : Hub
    {
        public async Task SendGetImage(IList<ShareBoardImages> images)
        {          
            await Clients.All.SendAsync("ReceiveImage", images);
           
        }
    }
}
