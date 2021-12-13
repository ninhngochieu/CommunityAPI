using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendAPI.DTO;
using BackendAPI.Extentions;
using BackendAPI.Models;
using BackendAPI.Modules;
using BackendAPI.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Repository.Implement
{
    public class LikeRepository: ILikeRepository
    {
        private readonly CommunityContext _context;

        public LikeRepository(CommunityContext context)
        {
            _context = context;
        }

        public async Task<UserLike> GetUserLike(Guid sourceUserId, Guid likeUserId)
        {
            return await _context.Likes.FindAsync(likeUserId, sourceUserId);
        }

        public async Task<AppUser> GetUserWithLikesInclude(Guid userId)
        {
            return await _context.AppUsers.Include(x => x.LikedUsers).FirstOrDefaultAsync(x => x.Id == userId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikeParams likeParams)
        {
            var users = _context.AppUsers.OrderBy(u => u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            switch (likeParams.Predicate) // Lấy những người tương tác với mình thông qua số like
            {
                case "liked":
                    likes = likes.Where(like => like.SourceUserId == likeParams.UserId); // TÌm nhưng dòng là mình like
                    users = likes.Select(like => like.LikedUser); // Lấy ra những AppUser like trong bảng này
                    break;
                case "likedBy":
                    likes = likes.Where(like => like.LikedUserId == likeParams.UserId);
                    users = likes.Select(like => like.SourceUser);
                    break;
                default: return await PagedList<LikeDto>.CreateNullListAsync();
            }

            var likeUsers = users.Select(u => new LikeDto
            {
                Username = u.UserName,
                Age = u.DateOfBirth.CalculateAge(),
                KnownAs = u.KnownAs,
                PhotoUrl = u.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = u.City,
                Id = u.Id
            });

            return await PagedList<LikeDto>.CreateAsync(likeUsers.AsNoTracking(), likeParams.PageNumber, likeParams.PageSize);
        }
    }
}