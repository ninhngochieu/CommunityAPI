using System.Threading.Tasks;
using BackendAPI.Extentions;
using BackendAPI.Models;
using BackendAPI.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers
{
    [Authorize]
    public class LikeController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly ILikeRepository _likeRepository;

        public LikeController(IUserRepository userRepository, ILikeRepository likeRepository)
        {
            _userRepository = userRepository;
            _likeRepository = likeRepository;
        }

        // GET: api/Like
        [HttpGet]
        public async Task<ActionResult> GetUserLikes(string predicate)
        {
            return OkResponse(await _likeRepository.GetUserLikes(predicate, User.GetUserId()));
        }

        // GET: api/Like/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Like
        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId(); // Lấy người like
            
            var likedUser = await _userRepository.GetUserAsync(username); // Tìm người cần like
            
            var sourceUser = await _likeRepository.GetUserWithLikes(sourceUserId); // Tìm người like với list like 1 - n

            if (likedUser is null) return NotFoundResponse("Không tìm thấy user này");

            if (sourceUser.UserName == username) return BadRequestResponse("Bạn không thể like chính mình");

            //Lỗi
            var userLike = await _likeRepository.GetUserLike(sourceUserId, likedUser.Id); // Tìm lượt like của người này với người kia

            if (userLike is not null) return BadRequest("Bạn đã like user này rồi");

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id
            };
            
            sourceUser.LikedUsers.Add(userLike);

            if (await _userRepository.SaveAllAsync())
            {
                return OkResponse("Đã like");
            }

            return BadRequest("Có lỗi khi like user này");
        }

        protected virtual bool IsAuthorize(int sourceUserId)
        {
            return sourceUserId != 0;
        }

        // PUT: api/Like/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/Like/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
