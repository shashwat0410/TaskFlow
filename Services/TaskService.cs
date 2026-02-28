using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Interfaces;
using TaskFlow.Models;
using TaskFlow.ViewModels;

namespace TaskFlow.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;

        public TaskService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaskViewModel>> GetProjectTasksAsync(int projectId)
        {
            return await _context.Tasks
                .Include(t => t.AssignedTo)
                .Include(t => t.Comments)
                .Where(t => t.ProjectId == projectId)
                .OrderBy(t => t.SortOrder)
                .Select(t => MapToViewModel(t))
                .ToListAsync();
        }

        public async Task<TaskDetailViewModel?> GetTaskDetailAsync(int taskId, string userId)
        {
            var task = await _context.Tasks
                .Include(t => t.AssignedTo)
                .Include(t => t.Project).ThenInclude(p => p.Members).ThenInclude(m => m.User)
                .Include(t => t.Comments).ThenInclude(c => c.Author)
                .FirstOrDefaultAsync(t => t.Id == taskId &&
                    (t.Project.OwnerId == userId || t.Project.Members.Any(m => m.UserId == userId)));

            if (task == null) return null;

            return new TaskDetailViewModel
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                DueDate = task.DueDate,
                CreatedAt = task.CreatedAt,
                ProjectId = task.ProjectId,
                ProjectName = task.Project.Name,
                AssignedToId = task.AssignedToId,
                AssignedToName = task.AssignedTo?.FullName,
                Comments = task.Comments.OrderBy(c => c.CreatedAt).Select(c => new CommentViewModel
                {
                    Id = c.Id,
                    Content = c.Content,
                    AuthorName = c.Author.FullName,
                    CreatedAt = c.CreatedAt
                }).ToList(),
                ProjectMembers = task.Project.Members.Select(m => new MemberViewModel
                {
                    UserId = m.UserId,
                    FullName = m.User.FullName,
                    Email = m.User.Email!,
                    Role = m.Role
                }).ToList()
            };
        }

        public async Task<List<TaskViewModel>> GetUserTasksAsync(string userId)
        {
            return await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.Comments)
                .Where(t => t.AssignedToId == userId)
                .OrderBy(t => t.DueDate)
                .Select(t => MapToViewModel(t))
                .ToListAsync();
        }

        public async Task<ProjectTask> CreateTaskAsync(TaskViewModel model, string userId)
        {
            var maxOrder = await _context.Tasks
                .Where(t => t.ProjectId == model.ProjectId)
                .MaxAsync(t => (int?)t.SortOrder) ?? 0;

            var task = new ProjectTask
            {
                Title = model.Title,
                Description = model.Description,
                Status = model.Status,
                Priority = model.Priority,
                DueDate = model.DueDate,
                ProjectId = model.ProjectId,
                AssignedToId = model.AssignedToId,
                CreatedById = userId,
                SortOrder = maxOrder + 1,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<bool> UpdateTaskAsync(TaskViewModel model, string userId)
        {
            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == model.Id &&
                    (t.Project.OwnerId == userId || t.Project.Members.Any(m => m.UserId == userId)));

            if (task == null) return false;

            task.Title = model.Title;
            task.Description = model.Description;
            task.Status = model.Status;
            task.Priority = model.Priority;
            task.DueDate = model.DueDate;
            task.AssignedToId = model.AssignedToId;

            if (model.Status == Models.TaskStatus.Done && task.CompletedAt == null)
                task.CompletedAt = DateTime.UtcNow;
            else if (model.Status != Models.TaskStatus.Done)
                task.CompletedAt = null;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateTaskStatusAsync(int taskId, Models.TaskStatus status, string userId)
        {
            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId &&
                    (t.Project.OwnerId == userId || t.Project.Members.Any(m => m.UserId == userId)));

            if (task == null) return false;

            task.Status = status;
            if (status == Models.TaskStatus.Done) task.CompletedAt = DateTime.UtcNow;
            else task.CompletedAt = null;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTaskAsync(int id, string userId)
        {
            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id &&
                    (t.Project.OwnerId == userId || t.Project.Members.Any(m => m.UserId == userId)));

            if (task == null) return false;

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Comment> AddCommentAsync(int taskId, string content, string userId)
        {
            var comment = new Comment
            {
                TaskId = taskId,
                Content = content,
                AuthorId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        private static TaskViewModel MapToViewModel(ProjectTask t) => new()
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Status = t.Status,
            Priority = t.Priority,
            DueDate = t.DueDate,
            CreatedAt = t.CreatedAt,
            ProjectId = t.ProjectId,
            ProjectName = t.Project?.Name ?? "",
            AssignedToId = t.AssignedToId,
            AssignedToName = t.AssignedTo?.FullName,
            CommentCount = t.Comments?.Count ?? 0
        };
    }
}
