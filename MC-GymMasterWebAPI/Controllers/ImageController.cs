using MC_GymMasterWebAPI.Data;
using MC_GymMasterWebAPI.DTOs;
using MC_GymMasterWebAPI.Interface;
using MC_GymMasterWebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;



namespace MC_GymMasterWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IGymMasterService _gymMasterService;

        public ImageController(IGymMasterService gymMasterService)
        {
            _gymMasterService = gymMasterService;
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
        public async Task<ActionResult<List<ImageLikeDTO>>> GetLikedImage(int member)
        {
            var memberLikedImages = await _gymMasterService.GetLikedImage(member);
           if(memberLikedImages.Any())
                return Ok(memberLikedImages);

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
            string s = "";
            if (like == null)
                return BadRequest(new { message = "No Image Like" });

            try
            {
                await _gymMasterService.UploadImageLike(like);
                return Ok(new { message = "Image Like successfully added." }); // Return as JSON object
            }
            catch (Exception ex)
            {
                // Log the exception (use a logger if available)
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }
  
    }
}
