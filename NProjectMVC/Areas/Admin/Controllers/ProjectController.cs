using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NProjectMVC.Data;
using NProjectMVC.Models;

namespace NProjectMVC.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class ProjectController : Controller
	{
		private readonly NProjectContext _context;

		public ProjectController(NProjectContext context)
		{
			_context = context;
		}

		// GET: Admin/Project
		public async Task<IActionResult> Index()
		{
			return _context.Projects != null ?
						View(await _context.Projects.ToListAsync()) :
						Problem("Entity set 'NProjectContext.Projects'  is null.");
		}

		// GET: Admin/Project/Details/5
		public async Task<IActionResult> Details(Guid? id)
		{
			if (id == null || _context.Projects == null)
			{
				return NotFound();
			}

			var project = await _context.Projects
				.FirstOrDefaultAsync(m => m.Id == id);
			if (project == null)
			{
				return NotFound();
			}

			return View(project);
		}

		// GET: Admin/Project/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: Admin/Project/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,Name,Description,Deadline,EstimatedWorkTime,TimeSpent")] Project project)
		{
			if (ModelState.IsValid)
			{
				project.Id = Guid.NewGuid();
				_context.Add(project);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(project);
		}

		// GET: Admin/Project/Edit/5
		public async Task<IActionResult> Edit(Guid? id)
		{
			if (id == null || _context.Projects == null)
			{
				return NotFound();
			}

			var project = await _context.Projects.FindAsync(id);
			if (project == null)
			{
				return NotFound();
			}
			return View(project);
		}

		// POST: Admin/Project/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Description,Deadline,EstimatedWorkTime,TimeSpent")] Project project)
		{
			if (id != project.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(project);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ProjectExists(project.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			return View(project);
		}

		// GET: Admin/Project/Delete/5
		public async Task<IActionResult> Delete(Guid? id)
		{
			if (id == null || _context.Projects == null)
			{
				return NotFound();
			}

			var project = await _context.Projects
				.FirstOrDefaultAsync(m => m.Id == id);
			if (project == null)
			{
				return NotFound();
			}

			return View(project);
		}

		// POST: Admin/Project/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(Guid id)
		{
			if (_context.Projects == null)
			{
				return Problem("Entity set 'NProjectContext.Projects'  is null.");
			}
			var project = await _context.Projects.FindAsync(id);
			if (project != null)
			{
				_context.Projects.Remove(project);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool ProjectExists(Guid id)
		{
			return (_context.Projects?.Any(e => e.Id == id)).GetValueOrDefault();
		}
	}
}
