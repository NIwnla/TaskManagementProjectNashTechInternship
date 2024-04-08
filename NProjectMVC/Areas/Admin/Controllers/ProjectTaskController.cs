using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NProjectMVC.Data;
using NProjectMVC.Extension;
using NProjectMVC.Interface;
using NProjectMVC.Models;
using NProjectMVC.Ultilities;

namespace NProjectMVC.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
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
				.ThenInclude(w => w.User)
				.Include(t => t.Project)
				.Include(t => t.AssignedMembers)
				.FirstOrDefault();
			if (task == null)
			{
				return NotFound("Detail not found");
			}
			return View(task);
		}

		public IActionResult Create(Guid? projectId)
		{
			if (!string.IsNullOrEmpty(projectId.ToString()))
			{
				var userInProject = _projectRepository
					.FindByCondition(p => p.Id == projectId)
					.Include(p => p.Members)
					.Select(p => p.Members)
					.FirstOrDefault();
				ViewData["AvailableUsers"] = userInProject;
				ViewData["ProjectId"] = projectId;
			}
			else
			{
				var projects = _projectRepository.FindAll();
				ViewData["ProjectIds"] = new SelectList(projects, "Id", "Name");
				ViewData["AvailableUsers"] = new List<User>();
			}
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(ProjectTask model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var memberIds = Request.Form["memberIds"];
			model.AssignedMembers = new List<User>();
			var currentUser = _userManager.Users.Where(u => u.Id == User.GetUserId()).FirstOrDefault();
			model.AssignedMembers.Add(currentUser);
			foreach (var memberId in memberIds)
			{
				var user = _userManager.Users.Where(u => u.Id == memberId).FirstOrDefault();
				model.AssignedMembers.Add(user);
			}
			if (_repository.Create(model, User.GetUserId()))
			{
				return RedirectToAction("Details", "Project", new { id = model.ProjectId });
			}
			else
			{
				return Problem("Error when create task");
			}
		}

		public IActionResult Edit(Guid id)
		{
			var task = _repository.FindByCondition(t => t.Id == id).Include(t => t.AssignedMembers).FirstOrDefault();
			if (task == null)
			{
				return NotFound("Not found");
			}
			var userInProject = _projectRepository
				.FindByCondition(p => p.Id == task.ProjectId)
				.Include(p => p.Members)
				.Select(p => p.Members)
				.FirstOrDefault();
			foreach (var assignedMember in task.AssignedMembers)
			{
				userInProject.Remove(assignedMember);
			}
			if (userInProject.Count > 0)
			{
				ViewData["UsersInProject"] = userInProject;
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
			var memberIds = Request.Form["memberIds"];
			var task = _repository.FindByCondition(t => t.Id == model.Id).Include(t => t.AssignedMembers).FirstOrDefault();
			task.Deadline = model.Deadline;
			task.Description = model.Description;
			task.EstimatedWorkTime = model.EstimatedWorkTime;
			task.Title = model.Title;
			task.Status = model.Status;
			foreach (var memberId in memberIds)
			{
				var user = _userManager.Users.Where(u => u.Id == memberId).FirstOrDefault();
				task.AssignedMembers.Add(user);
			}
			if (_repository.Update(task, User.GetUserId()))
			{
				var userInProject = _projectRepository
				.FindByCondition(p => p.Id == task.ProjectId)
				.Include(p => p.Members)
				.Select(p => p.Members)
				.FirstOrDefault();
				foreach (var assignedMember in task.AssignedMembers)
				{
					userInProject.Remove(assignedMember);
				}
				if (userInProject.Count > 0)
				{
					ViewData["UsersInProject"] = userInProject;
				}
				return View(task);
			}
			else
			{
				return Problem("Error when update task");
			}
		}

		public IActionResult RemoveMember(string memberId, Guid taskId)
		{
			var member = _userManager.Users.Where(u => u.Id == memberId).FirstOrDefault();
			var task = _repository.FindByCondition(t => t.Id == taskId).Include(t => t.AssignedMembers).FirstOrDefault();
			task.AssignedMembers.Remove(member);
			if (_repository.Update(task, User.GetUserId()))
			{
				return RedirectToAction("Edit", new { id = task.Id });
			}
			else
			{
				Console.WriteLine("Error while removing member from project");
				return RedirectToAction("Edit", new { id = task.Id });
			}
		}
	}
}
