using MC_GymMasterWebAPI.Data;
using MC_GymMasterWebAPI.DTOs;
using MC_GymMasterWebAPI.HubConfig;
using MC_GymMasterWebAPI.Interface;
using MC_GymMasterWebAPI.Models;
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


        public ImageController(IGymMasterService gymMasterService, IHubContext<SHub> hubContext)
        {
            _gymMasterService = gymMasterService;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<ActionResult<ShareBoard>> GetEveryMemberImage()
        {
            var memberImages = await _gymMasterService.GetEveryMemberImage();

            if (memberImages != null && memberImages.Any())
            {
                return Ok(memberImages);
            }

            return NotFound();


        }
        [HttpGet("likeCount")]
        public async Task<IActionResult> GetImageLikeCount()
        {
            var imageCounts = await _gymMasterService.GetLikedImage();

            if (imageCounts == null)
            {
                return NotFound("Image not found");
            }         

         
            await _hubContext.Clients.All.SendAsync("ReceiveLikeCountUpdate", imageCounts);

            string s = "";
            return Ok(imageCounts);
        }

        [HttpGet("memberId")]
        public async Task<ActionResult<List<ShareBoardImages>>> GetMemberImage(int memberId)
        {

            var memberImages = await _gymMasterService.GetMemberImage(memberId);

            if (memberImages != null && memberImages.Any())
            {
                return Ok(memberImages);
            }

            return NotFound();
        }
        [HttpGet("member")]
        public async Task<ActionResult<List<ImageLikeDTO>>> GetLikedImage(string member)
        {
           var memberLikedImages = await _gymMasterService.GetLikedImage(member);
           if(memberLikedImages.Any())
                return Ok(memberLikedImages);

            return NotFound();
        }
        [HttpDelete("Delete")]
        public async Task<ActionResult<ShareBoard>> DeleteImage(int shareBoardId)
        {
            string s = "";
            var deleteImage = await _gymMasterService.DeleteImage(shareBoardId);

            if (deleteImage != null)
            {
                return Ok(deleteImage); 
            }

            return NotFound(); 
        }

        [HttpPost("upload")]
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
                // Log the exception (use a logger if available)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("uploadImageLike")]
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
