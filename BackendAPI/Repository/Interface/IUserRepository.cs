using System.Threading.Tasks;
using BackendAPI.Controllers;
using BackendAPI.DTO;
using BackendAPI.Models;

namespace BackendAPI.Repository.Interface
{
    public interface IUserRepository
    {
        Task<MemberDto[]> GetUsersAsync();
        Task<MemberDto> GetUserDtoAsync(string username);

        Task<bool> SaveAllAsync();
        Task<AppUser> GetUserAsync(string username);
        void UpdateUser(AppUser user);
    }
}