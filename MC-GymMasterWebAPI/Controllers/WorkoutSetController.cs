using MC_GymMasterWebAPI.Data;
using MC_GymMasterWebAPI.DTOs;

using MC_GymMasterWebAPI.Models;
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
                ExpirationDate = DateOnly.Parse("2999-12-31"),
                LastModified = DateOnly.FromDateTime(DateTime.Now),
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
            try
            {
                var partCounts = await _dbContext.Members
                    .Where(m => m.UserId == userId)
                    .Join(
                        _dbContext.WorkoutSets,
                        m => m.MemberId,
                        w => w.MemberId,
                        (m, w) => new { Member = m, WorkoutSet = w }
                    )
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
            catch (Exception ex)
            {
                // Log the exception (you can use any logging mechanism here)
                // For example: _logger.LogError(ex, "Error occurred while fetching workout part counts.");

                // Return a generic error response
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        [HttpGet("id")]
        public async Task<ActionResult<List<YearCountDTO>>> GetAnnualWorkoutStatus(string id)
        {
            string s = "";
            try
            {
                var result = await _dbContext.WorkoutSets
                    .Join(
                        _dbContext.Members,
                        w => w.MemberId,
                        m => m.MemberId,
                        (w, m) => new { w.CreationDate, m.UserId }
                    )
                    .Where(wm => wm.UserId == id)
                    .GroupBy(wm => new
                    {
                        Month = wm.CreationDate.Month,
                        Year = wm.CreationDate.Year
                    })
                    .Select(g => new YearCountDTO
                    {
                        Year = g.Key.Month, // Corrected from g.Key.Month to g.Key.Year
                        YearCount = g.Count()
                    })
                    .OrderBy(r => r.Year)
                    .ThenBy(r => r.YearCount)
                    .ToListAsync();

                if (result.Any())
                {
                    return Ok(result);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                // Log the exception (replace with your logging mechanism)
                // For example: _logger.LogError(ex, "Error occurred while fetching annual workout data.");

                // Return a generic error response
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
     

    }
}
