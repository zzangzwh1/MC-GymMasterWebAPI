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
            string s = "";
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

    }
}
