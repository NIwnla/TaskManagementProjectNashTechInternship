using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NProjectMVC.Extension;
using NProjectMVC.Interface;
using NProjectMVC.Models;
using NProjectMVC.Ultilities;
using System.Drawing.Printing;

namespace NProjectMVC.Areas.NormalUser.Controllers
{
	[Area("NormalUser")]
	[Authorize(Roles = "User")]
	public class ProjectController : Controller
	{
		private readonly IRepository<Project> _repository;
		private readonly UserManager<User> _userManager;
		public ProjectController(IRepository<Project> repository, UserManager<User> userManager)
		{
			_userManager = userManager;
			_repository = repository;
		}
		public async Task<IActionResult> Index(string searchString, string currentFilter, int? pageNumber)
		{
			var list = _repository.FindAll()
				.Include(p => p.Members)
				.Include(p => p.ProjectTasks)
				.AsQueryable();
			list = list.Where(p => p.Members.Any(m => m.Id == User.GetUserId()));
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
				list = list.Where(l => l.Name.Contains(searchString));
			}
			int pageSize = 5;
			return View(await PaginatedList<Project>.CreateAsync(list.AsNoTracking(), pageNumber ?? 1, pageSize));
		}

		public IActionResult Details(Guid id)
		{
			var project = _repository.FindByCondition(p => p.Id == id).Include(p => p.ProjectTasks).ThenInclude(t => t.AssignedMembers).FirstOrDefault();
			if (project == null)
			{
				return NotFound("Not found");
			}
			return View(project);
		}
	}
}
