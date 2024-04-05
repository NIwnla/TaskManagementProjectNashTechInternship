using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NProjectMVC.Areas.NormalUser.Controllers
{
	[Area("NormalUser")]
	[Authorize(Roles = "User")]
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
