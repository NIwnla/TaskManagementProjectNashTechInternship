using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NProjectMVC.Interface;
using NProjectMVC.Models;

namespace NProjectMVC.Areas.Manager.Controllers
{
	[Area("Manager")]
	[Authorize(Roles = "Manager")]
	public class ProjectTaskController : Controller
	{
        private readonly UserManager<User> _userManager;
        public ProjectTaskController(UserManager<User> userManager)
		{
			_userManager = userManager;
		}

	}
}
