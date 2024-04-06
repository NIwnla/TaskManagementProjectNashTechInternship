using Microsoft.AspNetCore.Identity;
using NProjectMVC.Interface;
using NProjectMVC.Models;
using NProjectMVC.Models.Enum;
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
        public static UserRoles GetRoleOfUser(this UserManager<User> userManager, IUserRepository userRepository, string userId)
        {
            return userRepository.GetUserRoles(userId);
        }
    }
}
