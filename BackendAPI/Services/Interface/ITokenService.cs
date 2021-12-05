using BackendAPI.Models;

namespace BackendAPI
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}