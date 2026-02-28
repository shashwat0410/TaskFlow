using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Interfaces;
using TaskFlow.Models;
using TaskFlow.ViewModels;

namespace TaskFlow.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;

        public ProjectService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProjectViewModel>> GetUserProjectsAsync(string userId)
        {
            var projects = await _context.Projects
                .Include(p => p.Tasks)
                .Include(p => p.Members)
                .Include(p => p.Owner)
                .Where(p => p.OwnerId == userId || p.Members.Any(m => m.UserId == userId))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return projects.Select(MapToViewModel).ToList();
        }

        public async Task<ProjectViewModel?> GetProjectByIdAsync(int id, string userId)
        {
            var project = await _context.Projects
                .Include(p => p.Tasks).ThenInclude(t => t.AssignedTo)
                .Include(p => p.Tasks).ThenInclude(t => t.Comments)
                .Include(p => p.Members).ThenInclude(m => m.User)
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == id &&
                    (p.OwnerId == userId || p.Members.Any(m => m.UserId == userId)));

            if (project == null) return null;

            var vm = MapToViewModel(project);
            vm.Tasks = project.Tasks
                .OrderBy(t => t.SortOrder)
                .Select(t => new TaskViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    Priority = t.Priority,
                    DueDate = t.DueDate,
                    CreatedAt = t.CreatedAt,
                    ProjectId = t.ProjectId,
                    ProjectName = project.Name,
                    AssignedToId = t.AssignedToId,
                    AssignedToName = t.AssignedTo?.FullName,
                    CommentCount = t.Comments.Count
                }).ToList();

            vm.Members = project.Members.Select(m => new MemberViewModel
            {
                UserId = m.UserId,
                FullName = m.User.FullName,
                Email = m.User.Email!,
                Role = m.Role
            }).ToList();

            return vm;
        }

        public async Task<Project> CreateProjectAsync(ProjectViewModel model, string userId)
        {
            var project = new Project
            {
                Name = model.Name,
                Description = model.Description,
                Color = model.Color,
                Status = model.Status,
                StartDate = model.StartDate,
                DueDate = model.DueDate,
                OwnerId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            // Add owner as Admin member
            _context.ProjectMembers.Add(new ProjectMember
            {
                ProjectId = project.Id,
                UserId = userId,
                Role = MemberRole.Admin
            });
            await _context.SaveChangesAsync();

            return project;
        }

        public async Task<bool> UpdateProjectAsync(ProjectViewModel model, string userId)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == model.Id && p.OwnerId == userId);

            if (project == null) return false;

            project.Name = model.Name;
            project.Description = model.Description;
            project.Color = model.Color;
            project.Status = model.Status;
            project.StartDate = model.StartDate;
            project.DueDate = model.DueDate;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProjectAsync(int id, string userId)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == userId);

            if (project == null) return false;

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsProjectMemberAsync(int projectId, string userId)
        {
            return await _context.Projects
                .AnyAsync(p => p.Id == projectId &&
                    (p.OwnerId == userId || p.Members.Any(m => m.UserId == userId)));
        }

        private static ProjectViewModel MapToViewModel(Project p) => new()
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Color = p.Color,
            Status = p.Status,
            StartDate = p.StartDate,
            DueDate = p.DueDate,
            OwnerName = p.Owner?.FullName ?? "",
            TotalTasks = p.Tasks.Count,
            CompletedTasks = p.Tasks.Count(t => t.Status == Models.TaskStatus.Done),
            ProgressPercent = p.TotalTasks == 0 ? 0 : Math.Round((double)p.Tasks.Count(t => t.Status == Models.TaskStatus.Done) / p.Tasks.Count * 100, 1)
        };
    }
}
