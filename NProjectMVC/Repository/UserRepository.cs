using NProjectMVC.Data;
using NProjectMVC.Interface;
using NProjectMVC.Models.Enum;
using System.Data;

namespace NProjectMVC.Repository
{
	public class UserRepository : IUserRepository
	{
		private readonly NProjectContext _context;
		public UserRepository(NProjectContext nProjectContext)
		{
			_context = nProjectContext;
		}

		public UserRoles GetUserRoles(string userId)
		{
			var roleid = _context.UserRoles.Where(ur => ur.UserId == userId).Select(ur => ur.RoleId).FirstOrDefault();
            if (roleid == null)
            {
				return UserRoles.User;
            }
            var role = _context.Roles.Find(roleid);
			UserRoles roleEnum;
			Enum.TryParse(role.Name, out roleEnum);
			return roleEnum;
		}
	}
}
