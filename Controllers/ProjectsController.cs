using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Interfaces;
using TaskFlow.Models;
using TaskFlow.ViewModels;

namespace TaskFlow.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectsController(IProjectService projectService,
            UserManager<ApplicationUser> userManager)
        {
            _projectService = projectService;
            _userManager = userManager;
        }

        // GET: /Projects
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;
            var projects = await _projectService.GetUserProjectsAsync(userId);
            return View(projects);
        }

        // GET: /Projects/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var project = await _projectService.GetProjectByIdAsync(id, userId);
            if (project == null) return NotFound();
            return View(project);
        }

        // GET: /Projects/Create
        public IActionResult Create() => View(new ProjectViewModel { StartDate = DateTime.Today });

        // POST: /Projects/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = _userManager.GetUserId(User)!;
            var project = await _projectService.CreateProjectAsync(model, userId);
            TempData["Success"] = $"Project '{project.Name}' created successfully!";
            return RedirectToAction("Details", new { id = project.Id });
        }

        // GET: /Projects/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var project = await _projectService.GetProjectByIdAsync(id, userId);
            if (project == null) return NotFound();
            return View(project);
        }

        // POST: /Projects/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProjectViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = _userManager.GetUserId(User)!;
            var success = await _projectService.UpdateProjectAsync(model, userId);
            if (!success) return Forbid();

            TempData["Success"] = "Project updated successfully!";
            return RedirectToAction("Details", new { id = model.Id });
        }

        // POST: /Projects/Delete/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            await _projectService.DeleteProjectAsync(id, userId);
            TempData["Success"] = "Project deleted.";
            return RedirectToAction("Index");
        }

        // API: GET /Projects/GetProjectsJson
        [HttpGet]
        public async Task<IActionResult> GetProjectsJson()
        {
            var userId = _userManager.GetUserId(User)!;
            var projects = await _projectService.GetUserProjectsAsync(userId);
            return Json(projects);
        }
    }
}
