using Microsoft.AspNetCore.Identity;
using NProjectMVC.Models;
using System.Security.Claims;

namespace NProjectMVC.Extension
{
    public static class UserManagerExtension
    {
        public static string GetRolesOfCurrentUser(this UserManager<User> userManager, ClaimsPrincipal user)
        {
            var roles = user.Claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault();
            return roles != null ? roles.Value : "";
        }
    }
}
