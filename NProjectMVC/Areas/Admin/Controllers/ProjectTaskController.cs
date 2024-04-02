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
	public class ProjectTaskController : Controller
	{
		private readonly NProjectContext _context;

		public ProjectTaskController(NProjectContext context)
		{
			_context = context;
		}

		// GET: Admin/ProjectTask
		public async Task<IActionResult> Index()
		{
			var nProjectContext = _context.ProjectTasks.Include(p => p.Project);
			return View(await nProjectContext.ToListAsync());
		}

		// GET: Admin/ProjectTask/Details/5
		public async Task<IActionResult> Details(Guid? id)
		{
			if (id == null || _context.ProjectTasks == null)
			{
				return NotFound();
			}

			var projectTask = await _context.ProjectTasks
				.Include(p => p.Project)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (projectTask == null)
			{
				return NotFound();
			}

			return View(projectTask);
		}

		// GET: Admin/ProjectTask/Create
		public IActionResult Create()
		{
			ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description");
			return View();
		}

		// POST: Admin/ProjectTask/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,ProjectId,Title,Description,Deadline,Status,EstimatedWorkTime,TimeSpent")] ProjectTask projectTask)
		{
			if (ModelState.IsValid)
			{
				projectTask.Id = Guid.NewGuid();
				_context.Add(projectTask);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description", projectTask.ProjectId);
			return View(projectTask);
		}

		// GET: Admin/ProjectTask/Edit/5
		public async Task<IActionResult> Edit(Guid? id)
		{
			if (id == null || _context.ProjectTasks == null)
			{
				return NotFound();
			}

			var projectTask = await _context.ProjectTasks.FindAsync(id);
			if (projectTask == null)
			{
				return NotFound();
			}
			ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description", projectTask.ProjectId);
			return View(projectTask);
		}

		// POST: Admin/ProjectTask/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Guid id, [Bind("Id,ProjectId,Title,Description,Deadline,Status,EstimatedWorkTime,TimeSpent")] ProjectTask projectTask)
		{
			if (id != projectTask.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(projectTask);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ProjectTaskExists(projectTask.Id))
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
			ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description", projectTask.ProjectId);
			return View(projectTask);
		}

		// GET: Admin/ProjectTask/Delete/5
		public async Task<IActionResult> Delete(Guid? id)
		{
			if (id == null || _context.ProjectTasks == null)
			{
				return NotFound();
			}

			var projectTask = await _context.ProjectTasks
				.Include(p => p.Project)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (projectTask == null)
			{
				return NotFound();
			}

			return View(projectTask);
		}

		// POST: Admin/ProjectTask/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(Guid id)
		{
			if (_context.ProjectTasks == null)
			{
				return Problem("Entity set 'NProjectContext.ProjectTasks'  is null.");
			}
			var projectTask = await _context.ProjectTasks.FindAsync(id);
			if (projectTask != null)
			{
				_context.ProjectTasks.Remove(projectTask);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool ProjectTaskExists(Guid id)
		{
			return (_context.ProjectTasks?.Any(e => e.Id == id)).GetValueOrDefault();
		}
	}
}
