using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrictipleExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }
    }
}
