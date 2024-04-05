using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NProjectMVC.Extension;
using NProjectMVC.Interface;
using NProjectMVC.Models;
using NProjectMVC.Ultilities;

namespace NProjectMVC.Areas.NormalUser.Controllers
{
	[Area("NormalUser")]
	[Authorize(Roles = "User")]
	public class ProjectTaskController : Controller
	{
		private readonly UserManager<User> _userManager;
		private readonly IRepository<ProjectTask> _repository;
		private readonly IRepository<Project> _projectRepository;
		public ProjectTaskController(UserManager<User> userManager, IRepository<ProjectTask> repository, IRepository<Project> projectRepository)
		{
			_userManager = userManager;
			_repository = repository;
			_projectRepository = projectRepository;
		}

		public async Task<IActionResult> Index(string statusSort, string searchString, string currentFilter, string currentStatus, int? pageNumber)
		{
			var list = _repository.FindAll()
				.Include(t => t.AssignedMembers)
				.AsQueryable();
			list = list.Where(p => p.AssignedMembers.Any(m => m.Id == User.GetUserId()));
			if (searchString != null)
			{
				pageNumber = 1;
			}
			else
			{
				searchString = currentFilter;
			}
			if (statusSort != null)
			{
				pageNumber = 1;
			}
			else
			{
				statusSort = currentStatus;
			}
			ViewData["CurrentFilter"] = searchString;
			ViewData["CurrentStatus"] = statusSort;
			if (!String.IsNullOrEmpty(searchString))
			{
				list = list.Where(l => l.Title.Contains(searchString));
			}
			if (!String.IsNullOrEmpty(statusSort))
			{
				list = list.Where(l => l.Status == int.Parse(statusSort));
			}

			int pageSize = 5;
			return View(await PaginatedList<ProjectTask>.CreateAsync(list.AsNoTracking(), pageNumber ?? 1, pageSize));
		}

		public IActionResult Details(Guid id)
		{
			var task = _repository.FindByCondition(t => t.Id == id)
				.Include(t => t.WorkedTasks)
				.Include(t => t.Project)
				.Include(t => t.AssignedMembers)
				.FirstOrDefault();
			if (task == null)
			{
				return NotFound("Detail not found");
			}
			if (task.WorkedTasks != null)
			{
				task.WorkedTasks = task.WorkedTasks.Where(w => w.UserId == User.GetUserId()).ToList();
			}
			return View(task);
		}

		public IActionResult Edit(Guid id)
		{
			var task = _repository.FindByCondition(t => t.Id == id).FirstOrDefault();
			if (task == null)
			{
				return NotFound("Not found");
			}
			return View(task);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(ProjectTask model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var task = _repository.FindByCondition(t => t.Id == model.Id).Include(t => t.AssignedMembers).FirstOrDefault();
			task.Deadline = model.Deadline;
			task.Description = model.Description;
			task.EstimatedWorkTime = model.EstimatedWorkTime;
			task.Title = model.Title;
			task.Status = model.Status;
			if (_repository.Update(task, User.GetUserId()))
			{
				return RedirectToAction("Details", "Project", new {id = task.ProjectId});
			}
			else
			{
				return Problem("Error when update task");
			}
		}
	}
}
