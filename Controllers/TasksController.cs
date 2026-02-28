using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Interfaces;
using TaskFlow.Models;
using TaskFlow.ViewModels;

namespace TaskFlow.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly IProjectService _projectService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TasksController(ITaskService taskService, IProjectService projectService,
            UserManager<ApplicationUser> userManager)
        {
            _taskService = taskService;
            _projectService = projectService;
            _userManager = userManager;
        }

        // GET: /Tasks/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var task = await _taskService.GetTaskDetailAsync(id, userId);
            if (task == null) return NotFound();
            return View(task);
        }

        // GET: /Tasks/MyTasks
        public async Task<IActionResult> MyTasks()
        {
            var userId = _userManager.GetUserId(User)!;
            var tasks = await _taskService.GetUserTasksAsync(userId);
            return View(tasks);
        }

        // GET: /Tasks/Create?projectId=5
        public async Task<IActionResult> Create(int projectId)
        {
            var userId = _userManager.GetUserId(User)!;
            var project = await _projectService.GetProjectByIdAsync(projectId, userId);
            if (project == null) return NotFound();

            var vm = new TaskViewModel
            {
                ProjectId = projectId,
                ProjectName = project.Name
            };
            ViewBag.Members = project.Members;
            return View(vm);
        }

        // POST: /Tasks/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = _userManager.GetUserId(User)!;
            await _taskService.CreateTaskAsync(model, userId);
            TempData["Success"] = "Task created successfully!";
            return RedirectToAction("Details", "Projects", new { id = model.ProjectId });
        }

        // POST: /Tasks/Edit
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TaskViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = _userManager.GetUserId(User)!;
            var success = await _taskService.UpdateTaskAsync(model, userId);
            if (!success) return Forbid();

            TempData["Success"] = "Task updated!";
            return RedirectToAction("Details", new { id = model.Id });
        }

        // POST: /Tasks/Delete/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int projectId)
        {
            var userId = _userManager.GetUserId(User)!;
            await _taskService.DeleteTaskAsync(id, userId);
            TempData["Success"] = "Task deleted.";
            return RedirectToAction("Details", "Projects", new { id = projectId });
        }

        // POST: /Tasks/AddComment
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int taskId, string content)
        {
            var userId = _userManager.GetUserId(User)!;
            await _taskService.AddCommentAsync(taskId, content, userId);
            return RedirectToAction("Details", new { id = taskId });
        }

        // =================== REST API Endpoints ===================

        // API: PATCH /Tasks/UpdateStatus
        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest request)
        {
            var userId = _userManager.GetUserId(User)!;
            if (!Enum.TryParse<Models.TaskStatus>(request.Status, out var status))
                return BadRequest(new { error = "Invalid status value" });

            var success = await _taskService.UpdateTaskStatusAsync(request.TaskId, status, userId);
            return success
                ? Ok(new { message = "Status updated", taskId = request.TaskId, status = request.Status })
                : NotFound(new { error = "Task not found" });
        }

        // API: GET /Tasks/GetByProject/5
        [HttpGet]
        public async Task<IActionResult> GetByProject(int id)
        {
            var tasks = await _taskService.GetProjectTasksAsync(id);
            return Json(tasks);
        }
    }

    public class UpdateStatusRequest
    {
        public int TaskId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
