using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NProjectMVC.Extension;
using NProjectMVC.Interface;
using NProjectMVC.Models;
using NProjectMVC.Ultilities;

namespace NProjectMVC.Areas.Manager.Controllers
{
	[Area("Manager")]
	[Authorize(Roles = "Manager")]
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

		public async Task<IActionResult> Index(string searchString, string currentFilter, int? pageNumber)
		{
			var list = _repository.FindAll()
				.Include(t=> t.AssignedMembers)
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
			ViewData["CurrentFilter"] = searchString;

			if (!String.IsNullOrEmpty(searchString))
			{
				list = list.Where(l => l.Title.Contains(searchString));
			}

			int pageSize = 5;
			return View(await PaginatedList<ProjectTask>.CreateAsync(list.AsNoTracking(), pageNumber ?? 1, pageSize));
		}

		public IActionResult Create(Guid? projectId)
		{
			if (!string.IsNullOrEmpty(projectId.ToString()))
			{
				ViewData["ProjectId"] = projectId;
			}
			else
			{
				var projects = _projectRepository.FindAll();
				ViewData["ProjectIds"] = new SelectList(projects, "Id", "Name");
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
            model.AssignedMembers.Add(_userManager.Users.Where(u => u.Id == User.GetUserId()).FirstOrDefault());
            foreach (var memberId in memberIds)
            {
                var user = _userManager.Users.Where(u => u.Id == memberId).FirstOrDefault();
                model.AssignedMembers.Add(user);
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

    }
}
