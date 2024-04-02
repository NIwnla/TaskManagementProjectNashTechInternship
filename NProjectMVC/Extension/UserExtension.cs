using System.Security.Claims;

namespace NProjectMVC.Extension
{
    public static class UserExtension
    {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }
}
