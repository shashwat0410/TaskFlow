using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Interfaces;
using TaskFlow.ViewModels;

namespace TaskFlow.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardViewModel> GetDashboardDataAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);

            var projects = await _context.Projects
                .Include(p => p.Tasks)
                .Include(p => p.Owner)
                .Where(p => p.OwnerId == userId || p.Members.Any(m => m.UserId == userId))
                .ToListAsync();

            var myTasks = await _context.Tasks
                .Include(t => t.Project)
                .Where(t => t.AssignedToId == userId)
                .OrderBy(t => t.DueDate)
                .Take(10)
                .ToListAsync();

            var totalTasks = projects.Sum(p => p.Tasks.Count);
            var completedTasks = projects.Sum(p => p.Tasks.Count(t => t.Status == Models.TaskStatus.Done));
            var overdueTasks = projects.Sum(p => p.Tasks.Count(t =>
                t.DueDate.HasValue && t.DueDate < DateTime.Today && t.Status != Models.TaskStatus.Done));

            var tasksByStatus = new Dictionary<string, int>
            {
                ["Todo"] = projects.Sum(p => p.Tasks.Count(t => t.Status == Models.TaskStatus.Todo)),
                ["In Progress"] = projects.Sum(p => p.Tasks.Count(t => t.Status == Models.TaskStatus.InProgress)),
                ["Review"] = projects.Sum(p => p.Tasks.Count(t => t.Status == Models.TaskStatus.Review)),
                ["Done"] = completedTasks
            };

            var tasksByPriority = new Dictionary<string, int>
            {
                ["Low"] = projects.Sum(p => p.Tasks.Count(t => t.Priority == Models.TaskPriority.Low)),
                ["Medium"] = projects.Sum(p => p.Tasks.Count(t => t.Priority == Models.TaskPriority.Medium)),
                ["High"] = projects.Sum(p => p.Tasks.Count(t => t.Priority == Models.TaskPriority.High)),
                ["Critical"] = projects.Sum(p => p.Tasks.Count(t => t.Priority == Models.TaskPriority.Critical))
            };

            return new DashboardViewModel
            {
                UserName = user?.FullName ?? "User",
                TotalProjects = projects.Count,
                TotalTasks = totalTasks,
                CompletedTasks = completedTasks,
                OverdueTasks = overdueTasks,
                TasksByStatus = tasksByStatus,
                TasksByPriority = tasksByPriority,
                RecentProjects = projects
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(5)
                    .Select(p => new ProjectViewModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Color = p.Color,
                        Status = p.Status,
                        OwnerName = p.Owner?.FullName ?? "",
                        TotalTasks = p.Tasks.Count,
                        CompletedTasks = p.Tasks.Count(t => t.Status == Models.TaskStatus.Done),
                        ProgressPercent = p.TotalTasks == 0 ? 0 : Math.Round((double)p.Tasks.Count(t => t.Status == Models.TaskStatus.Done) / p.Tasks.Count * 100, 1),
                        DueDate = p.DueDate
                    }).ToList(),
                MyTasks = myTasks.Select(t => new TaskViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Status = t.Status,
                    Priority = t.Priority,
                    DueDate = t.DueDate,
                    CreatedAt = t.CreatedAt,
                    ProjectId = t.ProjectId,
                    ProjectName = t.Project?.Name ?? ""
                }).ToList(),
                UpcomingDeadlines = myTasks
                    .Where(t => t.DueDate.HasValue && t.DueDate >= DateTime.Today && t.Status != Models.TaskStatus.Done)
                    .OrderBy(t => t.DueDate)
                    .Take(5)
                    .Select(t => new TaskViewModel
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Status = t.Status,
                        Priority = t.Priority,
                        DueDate = t.DueDate,
                        ProjectId = t.ProjectId,
                        ProjectName = t.Project?.Name ?? ""
                    }).ToList()
            };
        }
    }
}
