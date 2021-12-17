using System.Threading.Tasks;
using BackendAPI.Extentions;
using BackendAPI.Models;
using BackendAPI.Modules;
using BackendAPI.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers
{
    [Authorize]
    public class LikeController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;

        public LikeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Like
        [HttpGet]
        public async Task<ActionResult> GetUserLikes([FromQuery]LikeParams likeParams)
        {
            likeParams.UserId = User.GetUserId();

            var userLikes = await _unitOfWork.LikeRepository.GetUserLikes(likeParams);
            
            Response.AddPaginationHeader(userLikes.CurrentPage, userLikes.PageSize, userLikes.TotalCount, userLikes.TotalPages);

            return OkResponse(userLikes);
        }
        
        // POST: api/Like
        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId(); // Lấy người like
            
            var likedUser = await _unitOfWork.UserRepository.GetUserAsync(username); // Tìm người cần like
            
            var sourceUser = await _unitOfWork.LikeRepository.GetUserWithLikesInclude(sourceUserId); // Tìm người like với list like 1 - n

            if (likedUser is null) return NotFoundResponse("Không tìm thấy user này");

            if (sourceUser.UserName == username) return BadRequestResponse("Bạn không thể like chính mình");

            //Lỗi
            var userLike = await _unitOfWork.LikeRepository.GetUserLike(sourceUserId, likedUser.Id); // Tìm lượt like của người này với người kia

            if (userLike is not null) return BadRequestResponse("Bạn đã like user này rồi");

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id
            };
            
            sourceUser.LikedUsers.Add(userLike);

            if (await _unitOfWork.Complete())
            {
                return OkResponse("Đã like");
            }

            return BadRequest("Có lỗi khi like user này");
        }
        
    }
}
