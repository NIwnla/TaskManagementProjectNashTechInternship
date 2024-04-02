using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NProjectMVC.Interface;
using NProjectMVC.Models;

namespace NProjectMVC.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = "Manager")]
    public class UserController : Controller
    {
        private readonly IRepository<User> _repository;
        public UserController(IRepository<User> repository)
        {
            _repository = repository;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
