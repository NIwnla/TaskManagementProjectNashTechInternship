using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NProjectMVC.Interface;
using NProjectMVC.Models;

namespace NProjectMVC.Controllers
{
    public class ProjectController : Controller
    {
        private readonly IRepository<Project> _repository;
        public ProjectController(IRepository<Project> repository)
        {
            _repository = repository;
        }

        public ActionResult Index()
        {
            var list = _repository.FindAll().ToList();
            return View(list);
        }

        // GET: ProjectController/Details/5
        public ActionResult Details(int id)
        {
            var project = _repository.FindByCondition(x => x.Id == x.Id).FirstOrDefault();
            if (project == null)
            {
                return NotFound("Cant find project");
            }
            return View(project);
        }

        // GET: ProjectController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProjectController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProjectController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProjectController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProjectController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProjectController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
