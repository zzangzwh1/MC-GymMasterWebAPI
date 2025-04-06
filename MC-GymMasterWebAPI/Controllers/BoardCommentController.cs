using MC_GymMasterWebAPI.DTOs;
using MC_GymMasterWebAPI.HubConfig;
using MC_GymMasterWebAPI.Interface;
using MC_GymMasterWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace MC_GymMasterWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardCommentController : Controller
    {
        private readonly IGymMasterService _gymMasterService;
        private readonly IHubContext<CommentHub> _hubContext;

        public BoardCommentController(IGymMasterService gymMasterService, IHubContext<CommentHub> hubContext)
        {
            _gymMasterService = gymMasterService;
            _hubContext = hubContext;
        }
        [HttpPost("AddComment")]
        public async Task<IActionResult> AddComment([FromBody] BoardCommentDTO comments)
        {
            if (comments == null || string.IsNullOrWhiteSpace(comments.Comment) || string.IsNullOrEmpty(comments.MemberId) || comments.ShareBoardId <= 0)
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
        public async Task<ActionResult<IList<MemberAndCommentInfoDTO>>> GetComments()
        {
            var comments = await _gymMasterService.GetComments();
            await _hubContext.Clients.All.SendAsync("ReceiveComment", comments);
            string test = "";
            if (comments != null)
                return Ok(comments);

            return NotFound();

        }

    }
}
