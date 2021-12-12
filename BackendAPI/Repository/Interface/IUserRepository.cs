using System;
using System.Threading.Tasks;
using BackendAPI.Controllers;
using BackendAPI.DTO;
using BackendAPI.Models;
using BackendAPI.Modules;

namespace BackendAPI.Repository.Interface
{
    public interface IUserRepository
    {
        Task<PagedList<MemberDto>> GetUsersAsync(UserParams @params);
        Task<MemberDto> GetUserDtoAsync(string username);

        Task<bool> SaveAllAsync();
        Task<AppUser> GetUserAsync(string username);
        void UpdateUser(AppUser user);
        Task<AppUser> GetUserByIdAsync(Guid userId);
    }
}