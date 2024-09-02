using MC_GymMasterWebAPI.Data;
using MC_GymMasterWebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;



namespace MC_GymMasterWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageUploadController : ControllerBase
    {
        private readonly GymMasterContext _dbContext;
        public ImageUploadController(GymMasterContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile image, [FromForm] int memberId)
        {
            string s = "";
            if (image == null || image.Length == 0)
                return BadRequest("No file uploaded.");

            using var memoryStream = new MemoryStream();
            await image.CopyToAsync(memoryStream);
            var imageBytes = memoryStream.ToArray();

            var shareBoard = new Models.ShareBoard
            {
                MemberId = memberId,
                ProfileImage = imageBytes,
                CreationDate = DateOnly.FromDateTime(DateTime.Now),
                LastModified = DateOnly.FromDateTime(DateTime.Now),
                ExpirationDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30)) // Example expiration
            };

            _dbContext.ShareBoards.Add(shareBoard);
            await _dbContext.SaveChangesAsync();

            return Ok("Image uploaded successfully.");
        }

    }
}
