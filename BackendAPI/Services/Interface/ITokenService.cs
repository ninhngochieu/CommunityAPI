using System.Threading.Tasks;
using BackendAPI.Models;

namespace BackendAPI.Services.Interface
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}