
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

        #region WorkoutSet
        public async Task<List<PartCountDTO>> GetMemberWorkoutPartCountsAsync(string userId)
        {
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
            return partCounts;
        }

        public async Task<List<YearCountDTO>> GetAnnualWorkoutStatusAsync(string id)
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
            return result;
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
        #endregion

        #region Member
        public async Task<IEnumerable<Member>> GetAllMembers()
        {
            return await _dbContext.Members.ToListAsync();
        }

        public async Task<Member> GetMemberByUsername(string userId)
        {
            return await _dbContext.Members.Where(m => m.UserId == userId)
                                         .FirstOrDefaultAsync();
        }

        public async Task<Member> GetMemberIdByUserId(string memberId)
        {
            return await _dbContext.Members
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
        #endregion


        #region Image
        public async Task<List<ShareBoardImages>> GetMemberImage(int memberId)
        {
            return await _dbContext.ShareBoards
                             .Where(m => m.ExpirationDate > DateOnly.FromDateTime(DateTime.Now) && m.MemberId == memberId)
                             .Select(m => new ShareBoardImages
                             {
                                 ShareBoardId = m.ShareBoardId,
                                 MemberId = m.MemberId,
                                 ProfileImage = m.ProfileImage != null ? $"data:image/png;base64,{Convert.ToBase64String(m.ProfileImage)}" : null,
                                 CreationDate = m.CreationDate,
                                 ExpirationDate = m.ExpirationDate,
                                 LastModified = m.LastModified
                             }).OrderByDescending(m => m.CreationDate)
                             .ToListAsync();
        }
        public async Task<List<ImageLikeDTO>> GetLikedImage(int member)
        {
            return await _dbContext.ImageLikes.Where(i => i.MemberId == member && i.ExpirationDate > DateOnly.FromDateTime(DateTime.Now))
                    .Select(i => new ImageLikeDTO
                    {
                        MemberId = i.MemberId,
                        ShareBoardId = i.ShareBoardId,
                        ImageLike = i.Like

                    }).ToListAsync();
        }

        public async Task UploadImage(IFormFile image, int memberId)
        {
            if (image == null || image.Length == 0 || memberId <= 0)
                throw new ArgumentException("Invalid image or member ID.");

            try
            {
                using var memoryStream = new MemoryStream();
                await image.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();

                var shareBoard = new ShareBoard
                {
                    MemberId = memberId,
                    ProfileImage = imageBytes,
                    CreationDate = DateOnly.FromDateTime(DateTime.Now),
                    LastModified = DateOnly.FromDateTime(DateTime.Now),
                    ExpirationDate = DateOnly.FromDateTime(DateTime.Parse("2099-12-31")) // Example expiration
                };

                _dbContext.ShareBoards.Add(shareBoard);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the upload process
                throw new InvalidOperationException($"Error occurred while uploading image: {ex.Message}", ex);
            }
        }

        public async Task UploadImageLike(ImageLikeDTO like)
        {
            var existingImageLike = await _dbContext.ImageLikes
                                     .FirstOrDefaultAsync(i => i.MemberId == like.MemberId
                                   && i.ShareBoardId == like.ShareBoardId);

            if (existingImageLike != null)
            {
                if (like.ImageLike == 0)
                {
                    existingImageLike.CreationgDate = DateOnly.FromDateTime(DateTime.Now);
                    existingImageLike.ExpirationDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));
                    existingImageLike.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);
                    existingImageLike.Like = 0;

                    // Save changes to the database
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    existingImageLike.CreationgDate = DateOnly.FromDateTime(DateTime.Now);
                    existingImageLike.ExpirationDate = DateOnly.FromDateTime(DateTime.Parse("2099-12-31"));
                    existingImageLike.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);
                    existingImageLike.Like = 1;

                    // Save changes to the database
                    await _dbContext.SaveChangesAsync();

                }
            }
            else
            {
                var likeImage = new ImageLike
                {
                    ShareBoardId = like.ShareBoardId,
                    MemberId = like.MemberId,
                    CreationgDate = DateOnly.FromDateTime(DateTime.Now),
                    ExpirationDate = DateOnly.FromDateTime(DateTime.Parse("2099-12-31")),
                    LastModifiedDate = DateOnly.FromDateTime(DateTime.Now),
                    Like = 1

                };
                _dbContext.ImageLikes.Add(likeImage);
                await _dbContext.SaveChangesAsync();
            }
        }


        public async Task<List<ShareBoardImages>> GetEveryMemberImage()
        {
            var memberImages = await _dbContext.ShareBoards
                .Where(m => m.ExpirationDate > DateOnly.FromDateTime(DateTime.Now))
                .Select(m => new ShareBoardImages
                {
                    ShareBoardId = m.ShareBoardId,
                    MemberId = m.MemberId,
                    ProfileImage = m.ProfileImage != null ? $"data:image/png;base64,{Convert.ToBase64String(m.ProfileImage)}" : null,
                    CreationDate = m.CreationDate,
                    ExpirationDate = m.ExpirationDate,
                    LastModified = m.LastModified
                })
                .OrderByDescending(m => m.CreationDate)
                .ToListAsync();

            return memberImages;
        }
        #endregion

        #region BoardComment
        public async Task<BoardComment> AddComment(BoardCommentDTO comments)
        {
            var boardComment = new BoardComment
            {
                Comment = comments.Comment,
                CreationgDate = DateOnly.FromDateTime(DateTime.Now),
                ExpirationDate = DateOnly.FromDateTime(new DateTime(2099, 12, 31)), // Explicit and clear date
                LastModifiedDate = DateOnly.FromDateTime(DateTime.Now),
                MemberId = comments.MemberId,
                ShareBoardId = comments.ShareBoardId
            };

            try
            {
                // Add the comment to the database
                _dbContext.BoardComments.Add(boardComment);
                await _dbContext.SaveChangesAsync();       
                return boardComment;
            }
            catch (Exception ex)
            {
                // Log the exception (replace with your logging framework if applicable)
                // _logger.LogError(ex, "Failed to add a comment.");
                throw new InvalidOperationException("An error occurred while adding the comment.", ex);
            }
        }
        #endregion
    }
}
