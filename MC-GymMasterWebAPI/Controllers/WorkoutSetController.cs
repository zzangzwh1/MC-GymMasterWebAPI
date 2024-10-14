using MC_GymMasterWebAPI.Data;
using MC_GymMasterWebAPI.DTOs;
using MC_GymMasterWebAPI.Interface;
using MC_GymMasterWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MC_GymMasterWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkoutSetController : ControllerBase
    {
        private readonly IGymMasterService _gymMasterService;

        public WorkoutSetController(IGymMasterService gymMasterService)
        {
            _gymMasterService = gymMasterService;
        }

        [HttpPost("insertWorkout")]
        public async Task<IActionResult> InsertWorkout([FromBody] List<WorkoutSetDTO> insertWorkout)
        {
           
       
            if (insertWorkout == null || insertWorkout.Count == 0)
            {
                return BadRequest("Invalid workout data.");
            }

            try
            {
                var savedWorkouts = await _gymMasterService.InsertWorkoutAsync(insertWorkout);
                return Ok(savedWorkouts);
            }
            catch (Exception ex)
            {
              
                return StatusCode(500, "An error occurred while saving the workout details.");
            }
        }

        [HttpGet("userId")]
        public async Task<ActionResult<List<PartCountDTO>>> GetMemberWorkoutPartCounts(string userId)
        {
          
            try
            {
                var partCounts = await _gymMasterService.GetMemberWorkoutPartCountsAsync(userId);

                if (partCounts.Any())
                {
                    return Ok(partCounts);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
              
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("id")]
        public async Task<ActionResult<List<YearCountDTO>>> GetAnnualWorkoutStatus(string id)
        {
            
            try
            {
                var result = await _gymMasterService.GetAnnualWorkoutStatusAsync(id);

                if (result.Any())
                {
                    return Ok(result);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
             
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
