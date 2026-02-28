using System.ComponentModel.DataAnnotations;
using TaskFlow.Models;

namespace TaskFlow.ViewModels
{
    // Auth ViewModels
    public class RegisterViewModel
    {
        [Required] public string FullName { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required, MinLength(6)] public string Password { get; set; } = string.Empty;
        [Compare("Password")] public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class LoginViewModel
    {
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }

    // Project ViewModels
    public class ProjectViewModel
    {
        public int Id { get; set; }
        [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
        [StringLength(500)] public string? Description { get; set; }
        public string Color { get; set; } = "#6366f1";
        public ProjectStatus Status { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime? DueDate { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public double ProgressPercent { get; set; }
        public List<TaskViewModel> Tasks { get; set; } = new();
        public List<MemberViewModel> Members { get; set; } = new();
    }

    // Task ViewModels
    public class TaskViewModel
    {
        public int Id { get; set; }
        [Required, StringLength(200)] public string Title { get; set; } = string.Empty;
        [StringLength(1000)] public string? Description { get; set; }
        public Models.TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string? AssignedToId { get; set; }
        public string? AssignedToName { get; set; }
        public int CommentCount { get; set; }
        public bool IsOverdue => DueDate.HasValue && DueDate < DateTime.Today && Status != Models.TaskStatus.Done;
    }

    public class TaskDetailViewModel : TaskViewModel
    {
        public List<CommentViewModel> Comments { get; set; } = new();
        public List<MemberViewModel> ProjectMembers { get; set; } = new();
        public string NewComment { get; set; } = string.Empty;
    }

    public class CommentViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string? AuthorAvatar { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class MemberViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public MemberRole Role { get; set; }
    }

    // Dashboard ViewModels
    public class DashboardViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public int TotalProjects { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int OverdueTasks { get; set; }
        public List<ProjectViewModel> RecentProjects { get; set; } = new();
        public List<TaskViewModel> MyTasks { get; set; } = new();
        public List<TaskViewModel> UpcomingDeadlines { get; set; } = new();
        public Dictionary<string, int> TasksByStatus { get; set; } = new();
        public Dictionary<string, int> TasksByPriority { get; set; } = new();
        public List<ActivityItem> RecentActivity { get; set; } = new();
    }

    public class ActivityItem
    {
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Icon { get; set; } = "bi-activity";
        public string Color { get; set; } = "primary";
    }
}
