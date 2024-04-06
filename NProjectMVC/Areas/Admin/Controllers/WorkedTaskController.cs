using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NProjectMVC.Interface;
using NProjectMVC.Models;
using NProjectMVC.Ultilities;

namespace NProjectMVC.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class WorkedTaskController : Controller
	{
		private readonly IRepository<WorkedTask> _repository;
		private readonly UserManager<User> _userManager;
		public WorkedTaskController(UserManager<User> userManager, IRepository<WorkedTask> repository)
		{
			_userManager = userManager;
			_repository = repository;
		}
		public async Task<IActionResult> Index(string searchString, string currentFilter, int? pageNumber)
		{
			var list = _repository.FindAll().Include(w => w.User).Include(w => w.ProjectTask).AsQueryable();
			if (searchString != null)
			{
				pageNumber = 1;
			}
			else
			{
				searchString = currentFilter;
			}
			ViewData["CurrentFilter"] = searchString;
			if (!String.IsNullOrEmpty(searchString))
			{
				list = list.Where(l => l.User.UserName.Contains(searchString)|| l.ProjectTask.Title.Contains(searchString));
			}

			int pageSize = 5;
			return View(await PaginatedList<WorkedTask>.CreateAsync(list.AsNoTracking(), pageNumber ?? 1, pageSize));
		}
	}
}
