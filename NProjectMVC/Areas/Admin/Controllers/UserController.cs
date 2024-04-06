using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NProjectMVC.Extension;
using NProjectMVC.Interface;
using NProjectMVC.Models;
using NProjectMVC.Models.ViewModel;
using NProjectMVC.Ultilities;

namespace NProjectMVC.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class UserController : Controller
	{
		private readonly UserManager<User> _userManager;
		private readonly IUserRepository _userRepository;
		public UserController(UserManager<User> userManager, IUserRepository userRepository)
		{
			_userManager = userManager;
			_userRepository = userRepository;
		}
		public IActionResult Index(string searchString, string currentFilter, int? pageNumber)
		{
			var list = _userManager.Users;
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
				list = list.Where(l => l.UserName.Contains(searchString));
			}
			var listVM = new List<UserVM>();
			foreach (var item in list.ToList())
			{
				var viewModel = new UserVM
				{
					Id = item.Id,
					UserName = item.UserName,
					Name = item.Name,
					DOB = item.DOB,
					Email = item.Email,
					Role = _userManager.GetRoleOfUser(_userRepository, item.Id)
				};
				listVM.Add(viewModel);
			}

			int pageSize = 5;
			return View(PaginatedList<UserVM>.CreateList(listVM, pageNumber ?? 1, pageSize));
		}

		[HttpPost]
		public async Task ChangeRole(string previousRole, string role, string userId)
		{
			var user = _userManager.Users.Where(u => u.Id == userId).FirstOrDefault();
			await _userManager.RemoveFromRoleAsync(user, previousRole);
			await _userManager.AddToRoleAsync(user, role);
		}
	}
}
