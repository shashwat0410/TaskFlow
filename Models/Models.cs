using Microsoft.AspNetCore.Identity;

namespace TaskFlow.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Project> OwnedProjects { get; set; } = new List<Project>();
        public ICollection<ProjectTask> AssignedTasks { get; set; } = new List<ProjectTask>();
        public ICollection<ProjectMember> ProjectMemberships { get; set; } = new List<ProjectMember>();
    }

    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Color { get; set; } = "#6366f1"; // default indigo
        public ProjectStatus Status { get; set; } = ProjectStatus.Active;
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string OwnerId { get; set; } = string.Empty;
        public ApplicationUser Owner { get; set; } = null!;

        public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
        public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();

        // Computed
        public int TotalTasks => Tasks.Count;
        public int CompletedTasks => Tasks.Count(t => t.Status == TaskStatus.Done);
        public double ProgressPercent => TotalTasks == 0 ? 0 : Math.Round((double)CompletedTasks / TotalTasks * 100, 1);
    }

    public class ProjectTask
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.Todo;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public int SortOrder { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public string? AssignedToId { get; set; }
        public ApplicationUser? AssignedTo { get; set; }

        public string CreatedById { get; set; } = string.Empty;

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }

    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int TaskId { get; set; }
        public ProjectTask Task { get; set; } = null!;

        public string AuthorId { get; set; } = string.Empty;
        public ApplicationUser Author { get; set; } = null!;
    }

    public class ProjectMember
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public MemberRole Role { get; set; } = MemberRole.Member;
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }

    public enum ProjectStatus { Active, OnHold, Completed, Archived }
    public enum TaskStatus { Todo, InProgress, Review, Done }
    public enum TaskPriority { Low, Medium, High, Critical }
    public enum MemberRole { Viewer, Member, Admin }
}
