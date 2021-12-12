using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using BackendAPI.DTO;
using BackendAPI.Models;

namespace BackendAPI.Repository.Interface
{
    public interface ILikeRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int likeUserId);
        Task<AppUser> GetUserWithLikes(int userId);
        Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, int userId);
    }
}