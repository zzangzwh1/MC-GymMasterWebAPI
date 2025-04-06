using MC_GymMasterWebAPI.DTOs;
using MC_GymMasterWebAPI.Models;
using Microsoft.AspNetCore.SignalR;
namespace MC_GymMasterWebAPI.HubConfig
{
    public class SHub :Hub
    {
        public async Task SendLikeCountUpdate(IList<ImageLikeCountDTO> imageCounts)
        {
            // Broadcast the updated like count to all connected clients
            await Clients.All.SendAsync("ReceiveLikeCountUpdate", imageCounts);
        }
    }
    public class CommentHub : Hub {
        public async Task SendAddComment(IList<MemberAndCommentInfoDTO> comments)
        {
            // Broadcast the updated like count to all connected clients
            await Clients.All.SendAsync("ReceiveComment", comments);
        }
    }
    public class ImageHub : Hub
    {
        public async Task SendGetImage(IList<ShareBoardImages> images)
        {
            // Broadcast the updated like count to all connected clients
            await Clients.All.SendAsync("ReceiveImage", images);
        }
    }
}
