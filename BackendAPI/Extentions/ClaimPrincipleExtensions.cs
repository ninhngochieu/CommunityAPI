using System;
using System.Security.Claims;

namespace BackendAPI.Extentions
{
    public static class ClaimPrincipleExtensions
    {
        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            return new Guid(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
        }
    }
}