using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NProjectMVC.Extension;
using NProjectMVC.Models;
using NProjectMVC.Models.Enum;
using System.Diagnostics;

namespace NProjectMVC.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly UserManager<User> _userManager;

		public HomeController(ILogger<HomeController> logger,UserManager<User> userManager)
		{
			_logger = logger;
			_userManager = userManager;
		}

		public IActionResult Index()
		{
            var role = _userManager.GetRolesOfCurrentUser(User);
            if (role == UserRoles.Admin.ToString())
            {
				return RedirectToAction("Index","Home", new {area = "Admin"});
            }
            if (role == UserRoles.Manager.ToString())
            {
                return RedirectToAction("Index", "Home", new { area = "Manager" });
            }
            if (role == UserRoles.User.ToString())
            {
                return RedirectToAction("Index", "Home", new { area = "NormalUser" });
            }
            return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
