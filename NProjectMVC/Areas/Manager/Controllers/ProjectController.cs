﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NProjectMVC.Extension;
using NProjectMVC.Interface;
using NProjectMVC.Models;
using NProjectMVC.Ultilities;
using NuGet.Packaging;
using NuGet.Packaging.Signing;

namespace NProjectMVC.Areas.Manager.Controllers
{
	[Area("Manager")]
	[Authorize(Roles = "Manager")]
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
			var project = _repository.FindByCondition(p => p.Id == id).Include(p => p.ProjectTasks).FirstOrDefault();
			if (project == null)
			{
				return NotFound("Not found");
			}
			return View(project);
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(Project model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var memberIds = Request.Form["memberIds"];
			model.Members = new List<User>();
			model.Members.Add(_userManager.Users.Where(u => u.Id == User.GetUserId()).FirstOrDefault());
			foreach (var memberId in memberIds)
			{
				var user = _userManager.Users.Where(u => u.Id == memberId).FirstOrDefault();
				model.Members.Add(user);
			}
			if (_repository.Create(model, User.GetUserId()))
			{
				return RedirectToAction(nameof(Index));
			}
			else
			{
				return Problem("Error when create project");
			}
		}

		public IActionResult Edit(Guid id)
		{
			var project = _repository.FindByCondition(p => p.Id == id).Include(p => p.Members).FirstOrDefault();
			var users = _userManager.Users.ToList();
			if (project == null)
			{
				return NotFound("Not found");
			}
			foreach (var member in project.Members)
			{
				users.Remove(member);
			}
			ViewData["UsersEdit"] = users;
			return View(project);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(Project model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var memberIds = Request.Form["memberIds"];
			var project = _repository.FindByCondition(p => p.Id == model.Id).Include(p => p.Members).FirstOrDefault();
			project.Deadline = model.Deadline;
			project.Description = model.Description;
			project.EstimatedWorkTime = model.EstimatedWorkTime;
			project.Name = model.Name;
			foreach (var memberId in memberIds)
			{
				var user = _userManager.Users.Where(u => u.Id == memberId).FirstOrDefault();
				project.Members.Add(user);
			}
			if (_repository.Update(project, User.GetUserId()))
			{
				return RedirectToAction(nameof(Index));
			}
			else
			{
				return Problem("Error when update project");
			}
		}

		public IActionResult RemoveMember(string memberId, Guid projectId)
		{
			var member = _userManager.Users.Where(u => u.Id == memberId).FirstOrDefault();
			var project = _repository.FindByCondition(p => p.Id == projectId).Include(p => p.Members).FirstOrDefault();
			project.Members.Remove(member);
			if (_repository.Update(project, User.GetUserId()))
			{
				return RedirectToAction("Edit", new { id = project.Id });
			}
			else
			{
				Console.WriteLine("Error while removing member from project");
				return RedirectToAction("Edit", new { id = project.Id });
			}
		}
	}
}
