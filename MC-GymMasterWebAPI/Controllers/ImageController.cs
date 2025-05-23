﻿using MC_GymMasterWebAPI.Data;
using MC_GymMasterWebAPI.DTOs;
using MC_GymMasterWebAPI.HubConfig;
using MC_GymMasterWebAPI.Interface;
using MC_GymMasterWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;



namespace MC_GymMasterWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IGymMasterService _gymMasterService;
        private readonly IHubContext<SHub> _hubContext;
      

        public ImageController(IGymMasterService gymMasterService, IHubContext<SHub> hubContext/*, IHubContext<ImageHub> hubImageContext*/)
        {
            _gymMasterService = gymMasterService;
            _hubContext = hubContext;         
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ShareBoard>> GetEveryMemberImage()
        {
            var memberImages = await _gymMasterService.GetEveryMemberImage();

            await _hubContext.Clients.All.SendAsync("ReceiveImage", memberImages);
            if (memberImages != null && memberImages.Any())
            {
                return Ok(memberImages);
            }

            return NotFound();


        }
        [HttpGet("getScrollDownImages")]
        [Authorize]
        public async Task<ActionResult<ShareBoardImages>> GetScrollDownImages(int shareBoardId, int page,string userId)
        {          
         
            var  memberImages = await _gymMasterService.GetScrollDownCurrentPageImages(shareBoardId,page, userId);                      

            await _hubContext.Clients.All.SendAsync("ReceiveImage", memberImages);
            if (memberImages != null && memberImages.Any())
            {
                return Ok(memberImages);
            }

            return NotFound();
        }
        [HttpGet("getScrollUpImages")]
        [Authorize]
        public async Task<ActionResult<ShareBoardImages>> GetScrollUpImages(int shareBoardId, int page,string userId)
        {
            var memberImages = await _gymMasterService.GetScrollUpCurrentPageImages(shareBoardId, page, userId);
  
            await _hubContext.Clients.All.SendAsync("ReceiveImage", memberImages);
            if (memberImages != null && memberImages.Any())
            {
                return Ok(memberImages);
            }

            return NotFound();
        }


        [HttpGet("memberId")]
        [Authorize]
        public async Task<ActionResult<List<ShareBoardImages>>> GetMemberImage(int memberId)
        {
            var memberImages = await _gymMasterService.GetMemberImage(memberId);

            if (memberImages != null && memberImages.Any())
            {
                return Ok(memberImages);
            }

            return NotFound();
        }
        [HttpPost("GetLikes")]
        [Authorize]
        public async Task<ActionResult<List<ImageLikeDTO>>> GetLikes(List<ShareBoardImages> images)
        {
           var memberLikedImages = await _gymMasterService.GetLikedImage(images);
            if (memberLikedImages.Any())
            {
                await _hubContext.Clients.All.SendAsync("ReceiveLikeCountUpdate", memberLikedImages);
                return Ok(memberLikedImages);
            }

            return NotFound();
        }
        [HttpDelete("Delete")]
        [Authorize]
        public async Task<ActionResult<ShareBoard>> DeleteImage(int shareBoardId)
        {          
            var deleteImage = await _gymMasterService.DeleteImage(shareBoardId);

            if (deleteImage != null)
            {
                return Ok(deleteImage); 
            }

            return NotFound(); 
        }

        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile image, [FromForm] int memberId)
        {

            if (image == null || image.Length == 0 || memberId <= 0)
                return BadRequest("No file uploaded.");           
            try
            {
                await _gymMasterService.UploadImage(image, memberId);
         
                return Ok("Image uploaded successfully.");
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }
       [HttpPost("uploadImageLike")]
       [Authorize]
        public async Task<IActionResult> UploadImageLike(ImageLikeDTO like)
        {      
           
            if (like.ShareBoardId >0 && !string.IsNullOrEmpty(like.UserId))
            {
                var result = await _gymMasterService.UploadImageLike(like);
                if(result == "success")
                {
                    return Ok(new Result{ Message = "success",IsSuccess=true });
                }
                return Ok(new Result { Message = "fail",IsSuccess=true});
            }
            return BadRequest(new Result { Message = "BAD" ,IsSuccess=false});
           
        }
  
    }
}
