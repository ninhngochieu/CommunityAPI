using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using BackendAPI.DTO;
using BackendAPI.Models;
using BackendAPI.Modules;

namespace BackendAPI.Repository.Interface
{
    public interface ILikeRepository
    {
        Task<UserLike> GetUserLike(Guid sourceUserId, Guid likeUserId);
        Task<AppUser> GetUserWithLikesInclude(Guid userId);
        Task<PagedList<LikeDto>> GetUserLikes(LikeParams likeParams);
    }
}