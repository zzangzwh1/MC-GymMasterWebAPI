using MC_GymMasterWebAPI.DTOs;
using MC_GymMasterWebAPI.HubConfig;
using MC_GymMasterWebAPI.Interface;
using MC_GymMasterWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace MC_GymMasterWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardCommentController : Controller
    {
        private readonly IGymMasterService _gymMasterService;
        private readonly IHubContext<SHub> _hubContext;

        public BoardCommentController(IGymMasterService gymMasterService, IHubContext<SHub> hubContext)
        {
            _gymMasterService = gymMasterService;
            _hubContext = hubContext;
        }
        [HttpPost("AddComment")]
        [AllowAnonymous]
        public async Task<ActionResult> AddComment([FromBody] BoardCommentDTO comments)
        {
            if (comments == null || string.IsNullOrWhiteSpace(comments.Comment) || string.IsNullOrEmpty(comments.MemberId) || comments.ShareBoardId <= 0)
            {
                return BadRequest(new { message = "Invalid comment data provided." });
            }
            try
            {
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
        [HttpPut("{boardCommendId}")]
        [AllowAnonymous]
        public async Task<ActionResult> EditComment(int boardCommendId, [FromBody] BoardCommentDTO comment)
        {

            if (boardCommendId <= 0)
            {
                return BadRequest("Mismatch between URL ID and comment ID.");
            }
            var updateComment = await _gymMasterService.EditComment(boardCommendId, comment);

            if (updateComment.IsSuccess)
            {
                return Ok(updateComment);
            }

            return BadRequest("BoardComment id is not exist");
        }
        [HttpPut("delete/{boardCommendId}")]
        [AllowAnonymous]
        public async Task<ActionResult> DeleteComment(int boardCommendId)
        {
            if (boardCommendId <= 0)
            {
                return BadRequest("Mismatch between URL ID and comment ID.");
            }
            var deleteComment = await _gymMasterService.DeleteComment(boardCommendId);
            if (deleteComment.IsSuccess)
            {
                return Ok(deleteComment);
            }

            return BadRequest("BoardComment id is not exist");
        }
        [HttpPost("GetComments")]
        [AllowAnonymous]
        public async Task<ActionResult<IList<MemberAndCommentInfoDTO>>> GetComments([FromBody] List<ShareBoardImages> images)
        {

            var comments = await _gymMasterService.GetComments(images);

            if (comments != null)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveComment", comments);
                return Ok(comments);
            }


            return NotFound();

        }

    }
}
