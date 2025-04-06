using MC_GymMasterWebAPI.Data;
using MC_GymMasterWebAPI.DTOs;
using MC_GymMasterWebAPI.Interface;
using MC_GymMasterWebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace MC_GymMasterWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly IGymMasterService _gymMasterService;

        public MemberController(IGymMasterService gymMasterService)
        {
            _gymMasterService = gymMasterService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Member>>> GetAllMembers()
        {

            var members = await _gymMasterService.GetAllMembers();
            if (members == null || !members.Any() )
            {
                return NotFound(); 
            }

            return Ok(members);
        }
        [HttpGet("userId")]
        public async Task<ActionResult<MemberDTO>> GetMemberByUsername(string userId)
        {

            var member = await _gymMasterService.GetMemberByUsername(userId);

            if (member != null)
            {
                return Ok(member);
            }

            return NotFound();
        }

        [HttpGet("memberId")]
        public async Task<ActionResult<Member>> GetMemberIdByUserId(string memberId)
        {

            var member = await _gymMasterService.GetMemberIdByUserId(memberId);

            if (member != null)
            {
                return Ok(member.MemberId);
            }

            return NotFound();
        }
        [HttpPost("edit")]
        public async Task<ActionResult<Member>> UpdateUserInfo([FromBody] MemberDTO memberDto)
        {          
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var member = await _gymMasterService.UpdateUserInfo(memberDto);

            if (member != null)
            {
                return Ok(member);
            }

            return NotFound();
        }



        [HttpPost("register")]
        public async Task<ActionResult<MemberDTO>> InsertMember([FromBody] MemberDTO memberDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
             
                var newMember = await _gymMasterService.InsertMember(memberDto);

                if (newMember == null)
                {
                    return StatusCode(500, "An error occurred while saving the member.");
                }

                return CreatedAtAction(nameof(GetMemberByUsername), new { userId = newMember.UserId }, newMember);
            }
            catch (Exception ex)
            {
              
                return StatusCode(500, "An error occurred while saving the member.");
            }
        }
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] LoginDto loginInfo)
        {        
            var user = await _gymMasterService.Authenticate(loginInfo);

           
            if (user == null)
            {
                return Unauthorized(new { success = false, message = "Invalid UserId" });
            }

            // Check password
            if (user.Password != loginInfo.Password)
            {
                return Unauthorized(new { success = false, message = "Invalid Password" });
            }
          

            return Ok(new { success = true, message = "Authentication successful" });
        }
       
        [HttpPost("requestPassword")]
        public async Task<IActionResult> RequestPassword([FromBody] RequestPasswordDTO requestPassword)
        {
            if (requestPassword.selectedValue != "email" || string.IsNullOrEmpty(requestPassword.userId))
            {
                return BadRequest("Invalid input data.");
            }

            try
            {
                var getMember = await _gymMasterService.GetMemberByUsername(requestPassword.userId);

                if (getMember == null)
                {
                    return NotFound("Member not found.");
                }

                var emailInfo = new MailData
                {
                    EmailSubject = "Gym Master - User Password",
                    EmailBody = @$"User Password: {getMember.Password} <br> If you have any questions or concerns, feel free to email - zzangzwh1@gmail.com.<br><br> Thanks,<br> Gym Master Support Team",
                    EmailToId = getMember.Email,
                    EmailToName = $"{getMember.FirstName} {getMember.LastName}",
                };

                var result = _gymMasterService.SendMail(emailInfo);

                return Ok(new { Message = "Email sent successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
            }
        }


    }
}
