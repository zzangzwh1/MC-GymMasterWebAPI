using MC_GymMasterWebAPI.DTOs;
using MC_GymMasterWebAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
namespace MC_GymMasterWebAPI.HubConfig
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SHub : Hub
    {
        public async Task SendLikeCountUpdate(IList<ImageLikeDTO> memberLikedImages)
        {

            await Clients.All.SendAsync("ReceiveLikeCountUpdate", memberLikedImages);
        }
        public async Task SendAddComment(IList<MemberAndCommentInfoDTO> comments)
        {

            await Clients.All.SendAsync("ReceiveComment", comments);
        }
        public async Task SendGetImage(IList<ShareBoardImages> images)
        {
            await Clients.All.SendAsync("ReceiveImage", images);

        }
    }
}
