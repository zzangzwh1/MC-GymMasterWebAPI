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

        [HttpPost]
        public async Task<ActionResult<Member>> InsertMember([FromBody] MemberDTO memberDto)
        {
            string s = "";
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



    }
}
