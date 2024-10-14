using MC_GymMasterWebAPI.Data;
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
        private readonly GymMasterContext _dbContext;
        public ImageController(GymMasterContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        public async Task<ActionResult<ShareBoard>> GetEveryMemberImage()
        {            
            var memberImages = await _dbContext.ShareBoards
                                        .Where(m => m.ExpirationDate > DateOnly.FromDateTime(DateTime.Now))
                                        .Select(m => new ShareBoardImages
                                        {
                                            ShareBoardId = m.ShareBoardId,
                                            MemberId = m.MemberId,
                                            ProfileImage = m.ProfileImage != null ? $"data:image/png;base64,{Convert.ToBase64String(m.ProfileImage)}" : null,                                           
                                            CreationDate = m.CreationDate,
                                            ExpirationDate = m.ExpirationDate,
                                            LastModified = m.LastModified
                                        }).OrderByDescending(m => m.CreationDate)
                                        .ToListAsync();

            if (memberImages != null && memberImages.Any())
            {
                return Ok(memberImages);
            }

            return NotFound();


        }

        [HttpGet("memberId")]
        public async Task<ActionResult<List<ShareBoardImages>>> GetMemberImage(int memberId)
        {
          
            var memberImages = await _dbContext.ShareBoards
                                       .Where (m=> m.ExpirationDate > DateOnly.FromDateTime(DateTime.Now) && m.MemberId == memberId)
                                       .Select(m => new ShareBoardImages
                                       {
                                           ShareBoardId = m.ShareBoardId,
                                           MemberId = m.MemberId,
                                           ProfileImage = m.ProfileImage != null ? $"data:image/png;base64,{Convert.ToBase64String(m.ProfileImage)}" : null,                                          
                                           CreationDate = m.CreationDate,
                                           ExpirationDate = m.ExpirationDate,
                                           LastModified = m.LastModified
                                       }).OrderByDescending(m=>m.CreationDate)
                                       .ToListAsync();

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
                using var memoryStream = new MemoryStream();
                await image.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();

                var shareBoard = new Models.ShareBoard
                {
                    MemberId = memberId,
                    ProfileImage = imageBytes,                   
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
