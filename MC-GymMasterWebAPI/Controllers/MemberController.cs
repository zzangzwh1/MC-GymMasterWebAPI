using MC_GymMasterWebAPI.Data;
using MC_GymMasterWebAPI.DTOs;
using MC_GymMasterWebAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace MC_GymMasterWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly GymMasterContext _dbContext;
        public MemberController(GymMasterContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Member>>> GetAllMembers()
        {
            var members = await (from m in _dbContext.Members
                                 select m).ToListAsync();
            if (members.Any())
            {
                return Ok(members);
            }

            return NotFound();
        }
        [HttpGet("userId")]
        public async Task<ActionResult<Member>> GetMemberByUsername(string userId)
        {
            var member = await _dbContext.Members
                                         .Where(m => m.UserId == userId)
                                         .FirstOrDefaultAsync();

            if (member != null)
            {
                return Ok(member);
            }

            return NotFound();
        }
        [HttpGet("memberId")]
        public async Task<ActionResult<Member>> GetMemberIdByUserId(string memberId)
        {
            var member = await _dbContext.Members
                                         .Where(m => m.UserId == memberId)
                                         .FirstOrDefaultAsync();

            if (member != null)
            {
                return Ok(member.MemberId);
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

            var member = new Member
            {
                UserId = memberDto.UserId,
                Address = memberDto.Address,
                BirthDate = memberDto.BirthDate,
                Email = memberDto.Email,
                Password = memberDto.Password,                
                CreationDate = DateTime.Now,
                ExpirationDate = DateTime.Parse("2099-12-31"),
                FirstName = memberDto.FirstName,
                LastName = memberDto.LastName,
                Phone = memberDto.Phone,
                Sex = memberDto.Sex
            };
            try
            {
                _dbContext.Members.Add(member);
                await _dbContext.SaveChangesAsync();
                return Ok(member);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here using your preferred logging framework
                return StatusCode(500, "An error occurred while saving the member.");
            }
        }
        [HttpPost("authenticate")]
    
        public async Task<IActionResult> Authenticate([FromBody] LoginDto loginInfo)
        {
            var user = await _dbContext.Members
                .FirstOrDefaultAsync(u => u.UserId == loginInfo.UserId);

            if (user == null)
            {
                return Unauthorized(new { success = false, message = "Invalid UserId" });
            }

            if (user.Password != loginInfo.Password)
            {
                return Unauthorized(new { success = false, message = "Invalid Password" });
            }

            // Assuming you generate and set a JWT token here
            // var token = GenerateJwtToken(user);
            // Response.Cookies.Append("AuthToken", token, new CookieOptions
            // {
            //     HttpOnly = true,
            //     Secure = true, // Ensure this is true in production for HTTPS
            //     SameSite = SameSiteMode.Strict // Adjust based on your needs
            // });

            return Ok(new { success = true, message = "Authentication successful" });
        }



    }
}
