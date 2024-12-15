using MC_GymMasterWebAPI.DTOs;
using MC_GymMasterWebAPI.Interface;
using MC_GymMasterWebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace MC_GymMasterWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardCommentController : Controller
    {
        private readonly IGymMasterService _gymMasterService;

        public BoardCommentController(IGymMasterService gymMasterService)
        {
            _gymMasterService = gymMasterService;
        }
        [HttpPost("AddComment")]
        public async Task<IActionResult> AddComment([FromBody] BoardCommentDTO comments)
        {
            if (comments == null || string.IsNullOrWhiteSpace(comments.Comment) || comments.MemberId <= 0 || comments.ShareBoardId <= 0)
            {
                return BadRequest(new { message = "Invalid comment data provided." });
            }

            try
            {
                // Call the service to add the comment
                var addedComment = await _gymMasterService.AddComment(comments);

                return Ok(new
                {
                    message = "Comment added successfully."
                
                });
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, new { message = "An error occurred while adding the comment.", error = ex.Message });
            }
        }
        [HttpGet("GetComments")]
        public async Task<ActionResult<IList<BoardComment>>> GetComments()
        {
            string s = "";
            var comments = await _gymMasterService.GetComments();
            if (comments != null)
                return Ok(comments);

            return NotFound();

        }

    }
}
