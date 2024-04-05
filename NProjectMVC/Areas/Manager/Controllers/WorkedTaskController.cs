using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NProjectMVC.Extension;
using NProjectMVC.Interface;
using NProjectMVC.Models;
using System;

namespace NProjectMVC.Areas.Manager.Controllers
{
	[Area("Manager")]
	[Authorize(Roles = "Manager")]
	public class WorkedTaskController : Controller
	{
		private readonly UserManager<User> _userManager;
		private readonly IRepository<WorkedTask> _repository;
		private readonly IRepository<ProjectTask> _taskRepository;
		private readonly IRepository<Project> _projectRepository;
		public WorkedTaskController(UserManager<User> userManager
			, IRepository<WorkedTask> repository
			, IRepository<ProjectTask> taskRepository
			, IRepository<Project> projectRepository)
		{
			_userManager = userManager;
			_repository = repository;
			_projectRepository = projectRepository;
			_taskRepository = taskRepository;
		}

		public IActionResult Index(Guid? taskId)
		{
			var list = _repository.FindByCondition(w => w.TaskId == taskId).OrderByDescending(w => w.StartTime).ToList();
			return View(list);
		}

		public IActionResult Create(Guid? taskId, Guid? projectId)
		{
			if (taskId != null) ViewData["TaskId"] = taskId;
			if (projectId != null) ViewData["ProjectId"] = projectId;
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(WorkedTask model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			if (_repository.Create(model, User.GetUserId()))
			{
				var task = _taskRepository.FindByCondition(t => t.Id == model.TaskId).FirstOrDefault();
				var project = _projectRepository.FindByCondition(p => p.Id == task.ProjectId).FirstOrDefault();
				task.EstimatedWorkTime = (float)Math.Clamp((double)task.EstimatedWorkTime - model.Time, 0, 9999);
				task.TimeSpent += model.Time;
				project.EstimatedWorkTime = (float)Math.Clamp((double)project.EstimatedWorkTime - model.Time, 0, 9999);
				project.TimeSpent += model.Time;
				if (!_projectRepository.Update(project, User.GetUserId())) return Problem("Error when create worked task");
				if (!_taskRepository.Update(task, User.GetUserId())) return Problem("Error when create worked task");
				return RedirectToAction("Details", "ProjectTask", new { id = model.TaskId });
			}
			else
			{
				return Problem("Error when create worked task");
			}
		}

		public IActionResult Edit(Guid id)
		{
			var work = _repository.FindByCondition(w => w.Id == id).FirstOrDefault();
			return View(work);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(WorkedTask model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var work = _repository.FindByCondition(w => w.Id == model.Id).FirstOrDefault();
			var task = _taskRepository.FindByCondition(t => t.Id == model.TaskId).FirstOrDefault();
			var project = _projectRepository.FindByCondition(p => p.Id == task.ProjectId).FirstOrDefault();
			task.TimeSpent = (float)Math.Clamp((double)task.TimeSpent - work.Time, 0, 9999);
			project.TimeSpent = (float)Math.Clamp((double)project.TimeSpent - work.Time, 0, 9999);
			task.TimeSpent += model.Time;
			project.TimeSpent += model.Time;
			work.Time = model.Time;
			work.Description = model.Description;
			work.StartTime = model.StartTime;
			if (_repository.Update(work, User.GetUserId()))
			{
				return RedirectToAction("Details", "ProjectTask", new { id = model.TaskId });
			}
			else
			{
				return Problem("Error when update worked task");
			}
		}
	}
}
