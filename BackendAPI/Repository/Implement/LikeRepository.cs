using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendAPI.DTO;
using BackendAPI.Extentions;
using BackendAPI.Models;
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

        public async Task<UserLike> GetUserLike(int sourceUserId, int likeUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, likeUserId);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.AppUsers.Include(x => x.LikedUsers).FirstOrDefaultAsync(x => x.Id == userId);
        }

        public async Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, int userId)
        {
            var users = _context.AppUsers.OrderBy(u => u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            switch (predicate) // Lấy những người tương tác với mình thông qua số like
            {
                case "liked":
                    likes = likes.Where(like => like.SourceUserId == userId); // TÌm nhưng dòng là mình like
                    users = likes.Select(like => like.LikedUser); // Lấy ra những AppUser like trong bảng này
                    break;
                case "likedBy":
                    likes = likes.Where(like => like.LikedUserId == userId);
                    users = likes.Select(like => like.LikedUser);
                    break;
                default: return new List<LikeDto>();
            }

            return await users.Select(u => new LikeDto
            {
                Username = u.UserName,
                Age = u.DateOfBirth.CalculateAge(),
                KnownAs = u.KnownAs,
                PhotoUrl = u.Photos.FirstOrDefault(p=>p.IsMain).Url,
                City = u.City,
                Id = u.Id
            }).ToListAsync();
        }
    }
}