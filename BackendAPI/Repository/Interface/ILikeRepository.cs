using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using BackendAPI.DTO;
using BackendAPI.Models;

namespace BackendAPI.Repository.Interface
{
    public interface ILikeRepository
    {
        Task<UserLike> GetUserLike(Guid sourceUserId, Guid likeUserId);
        Task<AppUser> GetUserWithLikes(Guid userId);
        Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, Guid userId);
    }
}