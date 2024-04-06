using NProjectMVC.Models.Enum;

namespace NProjectMVC.Interface
{
    public interface IUserRepository
    {
        UserRoles GetUserRoles(string userId);
    }
}
