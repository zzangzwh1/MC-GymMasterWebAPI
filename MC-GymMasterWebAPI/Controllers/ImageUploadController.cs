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
     
            if (image == null || image.Length == 0 || memberId <=0)
                return BadRequest("No file uploaded.");

            try
            {
                using var memoryStream = new MemoryStream();
                await image.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();

                var shareBoard = new Models.ShareBoard
                {
                    MemberId = memberId,
                    ProfileImage = imageBytes,
                    LikeImage = 0,
                    CreationDate = DateOnly.FromDateTime(DateTime.Now),
                    LastModified = DateOnly.FromDateTime(DateTime.Now),
                    ExpirationDate = DateOnly.FromDateTime(DateTime.Parse("2099-12-31")) // Example expiration
                };

                _dbContext.ShareBoards.Add(shareBoard);
                await _dbContext.SaveChangesAsync();

                return Ok("Image uploaded successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception (use a logger if available)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
