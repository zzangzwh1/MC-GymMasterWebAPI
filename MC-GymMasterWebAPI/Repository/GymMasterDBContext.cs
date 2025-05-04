using MC_GymMasterWebAPI.Data;
using MC_GymMasterWebAPI.DTOs;
using MC_GymMasterWebAPI.Interface;
using MC_GymMasterWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;


namespace MC_GymMasterWebAPI.Repository
{
    public class GymMasterDBContext : IGymMasterService
    {
        private readonly GymMasterContext _dbContext;
        private readonly MailSettings _mailSetting;
        public GymMasterDBContext(GymMasterContext dbContext, IOptions<MailSettings> options)
        {
            _dbContext = dbContext;
            _mailSetting = options.Value;
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
                        Year = g.Key.Month, 
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
        public async Task<Member> UpdateUserInfo(MemberDTO member)
        {

            var existingMember = await _dbContext.Members.FirstOrDefaultAsync(m => m.UserId == member.UserId);

            if (existingMember == null)
            {
                return null;
            }
            existingMember.Address = member.Address;
            existingMember.BirthDate = member.BirthDate;
            existingMember.Email = member.Email;
            existingMember.Password = member.Password;
            existingMember.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);
            existingMember.FirstName = member.FirstName;
            existingMember.LastName = member.LastName;
            existingMember.Phone = member.Phone;
            existingMember.Sex = member.Sex;

            // Save changes to the database
            await _dbContext.SaveChangesAsync();
            return existingMember;
        }

        public async Task<Member> GetMemberIdByUserId(string memberId)
        {
            return await _dbContext.Members.Where(m => m.UserId == memberId).FirstOrDefaultAsync();
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
            return await _dbContext.Members.FirstOrDefaultAsync(u => u.UserId == loginInfo.UserId);
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
        public async Task<List<ImageLikeDTO>> GetLikedImage(string member)
        {
            return await _dbContext.ImageLikes.Where(i => i.UserId == member && i.ExpirationDate > DateOnly.FromDateTime(DateTime.Now))
                    .Select(i => new ImageLikeDTO
                    {

                        ShareBoardId = i.ShareBoardId,
                        UserId = i.UserId
                    }).ToListAsync();
        }
        public async Task<List<ImageLikeCountDTO>> GetLikedImage()
        {
            var likedImageCounts = await _dbContext.ImageLikes
                .Where(i => i.ExpirationDate > DateOnly.FromDateTime(DateTime.Now))
                .GroupBy(i => i.ShareBoardId)
                .Select(group => new ImageLikeCountDTO
                {
                    ShareBoardId = group.Key,
                    TotalCount = group.Count()
                })
                .ToListAsync();

            return likedImageCounts;
        }
        public async Task<ShareBoard> DeleteImage(int shareBoardId)
        {
            var deleteImage = await _dbContext.ShareBoards
                                .FirstOrDefaultAsync(i => i.ShareBoardId == shareBoardId);
            if (deleteImage != null)
            {
                deleteImage.ExpirationDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));
                await _dbContext.SaveChangesAsync();
                return deleteImage;
            }

            return null;
        }
        public async Task<IList<ShareBoardImages>> GetScrollDownCurrentPageImages(int shardboardId, int page)
        {
            IList<ShareBoardImages> images = new List<ShareBoardImages>();
            if (shardboardId <= 0)
            {
                images = await _dbContext.ShareBoards
                .Where(m => m.ExpirationDate > DateOnly.FromDateTime(DateTime.Now))
                .OrderByDescending(m => m.ShareBoardId)
                .Take(page)
                .Select(m => new ShareBoardImages
                {
                    ShareBoardId = m.ShareBoardId,
                    MemberId = m.MemberId,
                    ProfileImage = m.ProfileImage != null ? $"data:image/png;base64,{Convert.ToBase64String(m.ProfileImage)}" : null,
                    CreationDate = m.CreationDate,
                    ExpirationDate = m.ExpirationDate,
                    LastModified = m.LastModified
                })
                .ToListAsync();
            }
            else {
                images = await _dbContext.ShareBoards
                .Where(m => m.ShareBoardId < shardboardId && m.ExpirationDate > DateOnly.FromDateTime(DateTime.Now))
                .OrderByDescending(m => m.ShareBoardId) 
                .Take(page)
                .Select(m => new ShareBoardImages
                {
                    ShareBoardId = m.ShareBoardId,
                    MemberId = m.MemberId,
                    ProfileImage = m.ProfileImage != null ? $"data:image/png;base64,{Convert.ToBase64String(m.ProfileImage)}" : null,
                    CreationDate = m.CreationDate,
                    ExpirationDate = m.ExpirationDate,
                    LastModified = m.LastModified
                })
                .ToListAsync();
                if (!images.Any() || images.Count< page) {

                    images = await _dbContext.ShareBoards
                        .Where(m=>m.ExpirationDate > DateOnly.FromDateTime(DateTime.Now))
                        .OrderBy(m => m.ShareBoardId)
                        .Take(page)
                        .Select(m => new ShareBoardImages
                        {
                            ShareBoardId = m.ShareBoardId,
                            MemberId = m.MemberId,
                            ProfileImage = m.ProfileImage != null ? $"data:image/png;base64,{Convert.ToBase64String(m.ProfileImage)}" : null,
                            CreationDate = m.CreationDate,
                            ExpirationDate = m.ExpirationDate,
                            LastModified = m.LastModified
                        })
                        .ToListAsync();
                }
          
            }

          
           

            return images;
        }
        public async Task<IList<ShareBoardImages>> GetScrollUpCurrentPageImages(int shardboardId, int page)
        {
            IList<ShareBoardImages> images = new List<ShareBoardImages>();
            if (shardboardId <= 0)
            {
                images = await _dbContext.ShareBoards
                .Where(m => m.ExpirationDate > DateOnly.FromDateTime(DateTime.Now))
                .OrderByDescending(m => m.ShareBoardId)
                .Take(page)
                .Select(m => new ShareBoardImages
                {
                    ShareBoardId = m.ShareBoardId,
                    MemberId = m.MemberId,
                    ProfileImage = m.ProfileImage != null ? $"data:image/png;base64,{Convert.ToBase64String(m.ProfileImage)}" : null,
                    CreationDate = m.CreationDate,
                    ExpirationDate = m.ExpirationDate,
                    LastModified = m.LastModified
                })
                .ToListAsync();
            }
            else
            {
                images = await _dbContext.ShareBoards
               .Where(m => m.ShareBoardId > shardboardId && m.ExpirationDate > DateOnly.FromDateTime(DateTime.Now))
               .OrderBy(m => m.ShareBoardId)
               .Take(page)
               .Select(m => new ShareBoardImages
               {
                   ShareBoardId = m.ShareBoardId,
                   MemberId = m.MemberId,
                   ProfileImage = m.ProfileImage != null ? $"data:image/png;base64,{Convert.ToBase64String(m.ProfileImage)}" : null,
                   CreationDate = m.CreationDate,
                   ExpirationDate = m.ExpirationDate,
                   LastModified = m.LastModified
               })
               .ToListAsync();
                if (!images.Any() || images.Count < page)
                {

                    images = await _dbContext.ShareBoards
                      .Where(m => m.ExpirationDate > DateOnly.FromDateTime(DateTime.Now))
                      .OrderByDescending(m => m.ShareBoardId)
                      .Take(page)
                      .Select(m => new ShareBoardImages
                      {
                          ShareBoardId = m.ShareBoardId,
                          MemberId = m.MemberId,
                          ProfileImage = m.ProfileImage != null ? $"data:image/png;base64,{Convert.ToBase64String(m.ProfileImage)}" : null,
                          CreationDate = m.CreationDate,
                          ExpirationDate = m.ExpirationDate,
                          LastModified = m.LastModified
                      })
                      .ToListAsync();
                }


            }

            return images;
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
                    ExpirationDate = DateOnly.FromDateTime(DateTime.Parse("2099-12-31")) 
                };

                _dbContext.ShareBoards.Add(shareBoard);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {                
                throw new InvalidOperationException($"Error occurred while uploading image: {ex.Message}", ex);
            }
        }

        public async Task<string> UploadImageLike(ImageLikeDTO like)
        {
            // Check if the ImageLike exists
            var existingImageLike = await _dbContext.ImageLikes
                .Where(i => i.UserId == like.UserId
                            && i.ShareBoardId == like.ShareBoardId
                          )
                .FirstOrDefaultAsync();

            ImageLike likeImage;
            string result = "";

            if (existingImageLike != null)
            {
                if (existingImageLike.ExpirationDate > DateOnly.FromDateTime(DateTime.Now))
                {
                    existingImageLike.ExpirationDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));
                    existingImageLike.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);

                    likeImage = existingImageLike;
                    result = "fail";
                }
                else
                {
                    existingImageLike.ExpirationDate = DateOnly.FromDateTime(DateTime.Parse("2099-12-31"));
                    existingImageLike.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);

                    likeImage = existingImageLike;
                    result = "success";
                }

            }
            else
            {
                likeImage = new ImageLike
                {
                    UserId = like.UserId,
                    ShareBoardId = like.ShareBoardId,
                    CreationDate = DateOnly.FromDateTime(DateTime.Now),
                    ExpirationDate = DateOnly.FromDateTime(DateTime.Parse("2099-12-31")),
                    LastModifiedDate = DateOnly.FromDateTime(DateTime.Now)
                };

                _dbContext.ImageLikes.Add(likeImage);
                result = "success";
            }

            try
            {

                await _dbContext.SaveChangesAsync();
                return result;
            }
            catch (Exception ex)
            {

                return "fail";
            }

        }

        public async Task<List<ShareBoardImages>> GetEveryMemberImage()
        {
           
           return await _dbContext.ShareBoards
                .Where(m => m.ExpirationDate > DateOnly.FromDateTime(DateTime.Now))
                .OrderByDescending(m => m.CreationDate)              
          
                .Select(m => new ShareBoardImages
                {
                    ShareBoardId = m.ShareBoardId,
                    MemberId = m.MemberId,
                    ProfileImage = m.ProfileImage != null ? $"data:image/png;base64,{Convert.ToBase64String(m.ProfileImage)}" : null,
                    CreationDate = m.CreationDate,
                    ExpirationDate = m.ExpirationDate,
                    LastModified = m.LastModified
                })
                .ToListAsync();      

        }
        #endregion

        #region BoardComment
        public async Task<BoardComment> AddComment(BoardCommentDTO comments)
        {
            var memberId = await _dbContext.Members.Where(i => i.UserId == comments.MemberId).Select(i => i.MemberId).FirstOrDefaultAsync();
            var boardComment = new BoardComment
            {
                Comment = comments.Comment,
                CreationgDate = DateOnly.FromDateTime(DateTime.Now),
                ExpirationDate = DateOnly.FromDateTime(new DateTime(2099, 12, 31)),
                LastModifiedDate = DateOnly.FromDateTime(DateTime.Now),
                MemberId = memberId,
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
                throw new InvalidOperationException("An error occurred while adding the comment.", ex);
            }
        }
        public async Task<Result> EditComment(int boardCommendId, BoardCommentDTO comment)
        {
            var boardComment = await _dbContext.BoardComments
                .FirstOrDefaultAsync(i => i.BoardCommentId == boardCommendId);
            if (boardComment != null)
            {
                boardComment.Comment = comment.Comment;
                boardComment.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);
                await _dbContext.SaveChangesAsync();

                return new Result { IsSuccess = true, Message = "success" };
            }
            return new Result
            {
                IsSuccess = false,
                Message = "fail"

            };
        }
        public async Task<Result> DeleteComment(int boardCommendId) {

            var boardComment = await _dbContext.BoardComments
            .FirstOrDefaultAsync(i => i.BoardCommentId == boardCommendId);
            if (boardComment != null)
            {
            
                boardComment.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);
                boardComment.ExpirationDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));
                await _dbContext.SaveChangesAsync();

                return new Result { IsSuccess = true, Message = "success" };
            }
            return new Result
            {
                IsSuccess = false,
                Message = "fail"

            };
        }
        public async Task<IList<MemberAndCommentInfoDTO>> GetComments(IList<ShareBoardImages> image)
        {
            var shareBoardIds = image.Select(img => img.ShareBoardId).ToList();
            
            var result = await (from m in _dbContext.Members
                                join b in _dbContext.BoardComments
                                on m.MemberId equals b.MemberId
                                where b.ExpirationDate > DateOnly.FromDateTime(DateTime.Now)
                                      && shareBoardIds.Contains(b.ShareBoardId) 
                                orderby b.BoardCommentId descending
                                select new MemberAndCommentInfoDTO
                                {
                                    MemberId = m.MemberId,
                                    Address = m.Address,
                                    Email = m.Email,
                                    FirstName = m.FirstName,
                                    LastName = m.LastName,
                                    Phone = m.Phone,
                                    UserId = m.UserId,
                                    ShareBoardId = b.ShareBoardId,
                                    Comment = b.Comment,
                                    BoardCommentId = b.BoardCommentId
                                }).ToListAsync();

            return result;


        }
        #endregion

        #region Email
        //smtpemailTest1
        public async Task<bool> SendMail(MailData email)
        {

            try
            {
                var smtpClient = new SmtpClient(_mailSetting.Host)
                {
                    Port = _mailSetting.Port,
                    Credentials = new NetworkCredential(_mailSetting.UserName, _mailSetting.Password),
                    EnableSsl = true,
                };
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_mailSetting.UserName, "GYM-Master"),
                    Subject = email.EmailSubject,
                    Body = email.EmailBody,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(email.EmailToId);
                await smtpClient.SendMailAsync(mailMessage);



                return true;

            }
            catch (Exception ex)
            {               
                return false;
            }
        }

        #endregion
    }
}
