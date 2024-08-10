using MC_GymMasterWebAPI.Data;
using MC_GymMasterWebAPI.DTOs;
using MC_GymMasterWebAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MC_GymMasterWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkoutSetController : Controller
    {
        private readonly GymMasterContext _dbContext;
        public WorkoutSetController(GymMasterContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpPost("WorkoutSet")]
        public async Task<ActionResult<WorkoutSetDTO>> InsertWorkoutSet([FromBody] WorkoutSetDTO workout)
        {
           
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var work = new WorkoutSet
            {
                CreationDate = workout.CreationDate,
                ExpirationDate = DateTime.Parse("2999-12-31"),
                LastModified = DateTime.Now,
                MemberId = workout.MemberId,
                Part = workout.Part,
                RepCount = workout.RepCount,
                Weight = workout.Weight,
                SetDescription = workout.SetDescription,
                SetCount = workout.SetCount,
            };


      
            try
            {
                _dbContext.WorkoutSets.Add(work);
                await _dbContext.SaveChangesAsync();
                return Ok(work);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here using your preferred logging framework
                return StatusCode(500, "An error occurred while saving the Workout Details.");
            }
        }
        [HttpGet("userId")]
        public async Task<ActionResult<List<PartCountDTO>>> GetMemberWorkoutPartCounts(string userId)
        {
            string s = "";
            var partCounts = await _dbContext.Members
                .Where(m => m.UserId == userId)
                .Join(_dbContext.WorkoutSets,
                      m => m.MemberId,
                      w => w.MemberId,
                      (m, w) => new { Member = m, WorkoutSet = w })
                .GroupBy(joined => joined.WorkoutSet.Part)
                .Select(group => new PartCountDTO
                {
                    Part = group.Key,
                    TotalCount = group.Count()
                })
                .ToListAsync();

            if (partCounts.Any())
            {
                return Ok(partCounts);
            }

            return NotFound();
        }

    }
}
