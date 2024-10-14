
using MC_GymMasterWebAPI.Data;
using MC_GymMasterWebAPI.DTOs;
using MC_GymMasterWebAPI.Interface;
using MC_GymMasterWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MC_GymMasterWebAPI.Repository
{
    public class GymMasterDBContext : IGymMasterService
    {
        private readonly GymMasterContext _dbContext;

        public GymMasterDBContext(GymMasterContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<PartCountDTO>> GetMemberWorkoutPartCountsAsync(string userId)
        {
            return await _dbContext.Members
                .Where(m => m.UserId == userId)
                .Join(
                    _dbContext.WorkoutSets,
                    m => m.MemberId,
                    w => w.MemberId,
                    (m, w) => new { m, w.Part }
                )
                .GroupBy(g => g.Part)
                .Select(g => new PartCountDTO
                {
                    Part = g.Key,
                    TotalCount = g.Count()
                })
                .ToListAsync();
        }

        public async Task<List<YearCountDTO>> GetAnnualWorkoutStatusAsync(string userId)
        {
            return await _dbContext.WorkoutSets
                .Join(
                    _dbContext.Members,
                    w => w.MemberId,
                    m => m.MemberId,
                    (w, m) => new { w.CreationDate, m.UserId }
                )
                .Where(wm => wm.UserId == userId)
                .GroupBy(wm => new { wm.CreationDate.Year })
                .Select(g => new YearCountDTO
                {
                    Year = g.Key.Year,
                    YearCount = g.Count()
                })
                .OrderBy(r => r.Year)
                .ToListAsync();
        }

        public async Task<List<WorkoutSetDTO>> InsertWorkoutAsync(List<WorkoutSetDTO> insertWorkout)
        {
            if (insertWorkout == null || !insertWorkout.Any())
            {
                throw new ArgumentException("Invalid workout data provided.");
            }

            var savedWorkouts = new List<WorkoutSetDTO>();

            foreach (var workout in insertWorkout)
            {
                // Input validation
                if (workout.MemberId <= 0)
                {
                    throw new ArgumentException("Invalid workout data provided.");
                }

                try
                {
                    var workoutSet = new WorkoutSet
                    {
                        CreationDate = workout.CreationDate,
                        ExpirationDate = DateOnly.Parse("2999-12-31"),
                        LastModified = DateOnly.FromDateTime(DateTime.Now),
                        MemberId = workout.MemberId,
                        Part = workout.Part,
                        RepCount = workout.RepCount,
                        Weight = workout.Weight,
                        SetDescription = workout.SetDescription,
                        SetCount = workout.SetCount
                    };

                    await _dbContext.WorkoutSets.AddAsync(workoutSet);
                    await _dbContext.SaveChangesAsync();

                    // Add the saved workout to the result list
                    savedWorkouts.Add(new WorkoutSetDTO
                    {
                        CreationDate = workoutSet.CreationDate,
                        ExpirationDate = workoutSet.ExpirationDate,
                        LastModified = workoutSet.LastModified,
                        MemberId = workoutSet.MemberId,
                        Part = workoutSet.Part,
                        RepCount = workoutSet.RepCount,
                        Weight = workoutSet.Weight,
                        SetDescription = workoutSet.SetDescription,
                        SetCount = workoutSet.SetCount
                    });
                }
                catch (DbUpdateException ex)
                {
                    throw new Exception("An error occurred while saving the workout set. Please try again.", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception("An unexpected error occurred.", ex);
                }
            }

            return savedWorkouts;
        }

        public async Task<IEnumerable<Member>> GetAllMembers()
        {
            return await _dbContext.Members.ToListAsync();
        }

        public async Task<Member> GetMemberByUsername(string userId)
        {
            return await _dbContext.Members.Where(m => m.UserId == userId)
                                         .FirstOrDefaultAsync();
        }

        public async  Task<Member> GetMemberIdByUserId(string memberId)
        {
           return  await _dbContext.Members
                                         .Where(m => m.UserId == memberId)
                                         .FirstOrDefaultAsync();
        }

        public async Task<MemberDTO> InsertMember(MemberDTO memberDto)
        {
            var member = new Member
            {
                UserId = memberDto.UserId,
                Address = memberDto.Address,
                BirthDate = memberDto.BirthDate,
                Email = memberDto.Email,
                Password = memberDto.Password,
                CreationDate = DateOnly.FromDateTime(DateTime.Now),
                ExpirationDate = DateOnly.FromDateTime(DateTime.Parse("2099-12-31")),
                FirstName = memberDto.FirstName,
                LastName = memberDto.LastName,
                Phone = memberDto.Phone,
                Sex = memberDto.Sex
            };

            _dbContext.Members.Add(member);
            await _dbContext.SaveChangesAsync();

            // Convert the entity back to DTO if necessary
            var newMemberDto = new MemberDTO
            {
                UserId = member.UserId,
                Address = member.Address,
                BirthDate = member.BirthDate,
                Email = member.Email,
                FirstName = member.FirstName,
                LastName = member.LastName,
                Phone = member.Phone,
                Sex = member.Sex
            };

            return newMemberDto;
        }


        public async Task<Member> Authenticate(LoginDto loginInfo)
        {
            // Find user by UserId
            var user = await _dbContext.Members
                .FirstOrDefaultAsync(u => u.UserId == loginInfo.UserId);

            // Return the user (null if not found)
            return user;
        }

     
    }
}
