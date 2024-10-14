using MC_GymMasterWebAPI.Data;
using MC_GymMasterWebAPI.Interface;
using MC_GymMasterWebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



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
        [HttpPost]
        public async Task<ActionResult> UploadImageLike(ImageLike like)
        {
            string s = "";
            if (like == null)
                return BadRequest("No Image Like");

            return null;
        }

    }
}
