using System.Security.Claims;

namespace BackendAPI.Extentions
{
    public static class ClaimPrincipleExtensions
    {
        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}