using MC_GymMasterWebAPI.DTOs;
using Microsoft.AspNetCore.SignalR;
namespace MC_GymMasterWebAPI.HubConfig
{
    public class SHub :Hub
    {
        public async Task SendLikeCountUpdate(List<ImageLikeCountDTO> imageCounts)
        {
            // Broadcast the updated like count to all connected clients
            await Clients.All.SendAsync("ReceiveLikeCountUpdate", imageCounts);
        }
    }
}
