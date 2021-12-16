using System.Linq;
using System.Threading.Tasks;
using BackendAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Controllers
{
    public class AdminController: BaseController
    {
        private readonly UserManager<AppUser> _userManager;

        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        // [Authorize(Policy="RequireAdminRole")]
        [Authorize(Policy="ModeratePhotoRole")]
        [HttpGet("Users-With-Roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await _userManager.Users
                .Include(r=>r.UserRoles)
                .ThenInclude(r=>r.Role)
                .OrderBy(u=>u.UserName)
                .Select(u => new 
                {
                    u.Id,
                    Username = u.UserName,
                    Roles = u.UserRoles.Select(r=>r.Role.Name).ToList(),
                })
                .ToListAsync();
            
            return OkResponse(users);
        }

        [HttpPost("Edit-Roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery]string roles)
        {
            var selectedRoles = roles.Split(",").ToArray();

            var user = await _userManager.FindByNameAsync(username);

            if (user is null)
            {
                return NotFoundResponse("Không tìm thấy User này");
            }
            
            var currentUserRoles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(currentUserRoles));

            if (!result.Succeeded)
            {
                return BadRequestResponse("Thất bại khi thêm role");
            }

            result = await _userManager.RemoveFromRolesAsync(user, currentUserRoles.Except(selectedRoles));

            if (!result.Succeeded)
            {
                return BadRequestResponse("Thất bại khi xoá role");
            }

            return OkResponse(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Policy="ModeratePhotoRole")]
        [HttpGet("Photos-To-Moderate")]
        public ActionResult GetPhotoForModeration()
        {
            return OkResponse("Chỉ có Administrator hoặc Moderator mới có thể thấy nội dung này");
        }
        
        


        
        
    }
}