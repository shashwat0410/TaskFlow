using TaskFlow.Models;
using TaskFlow.ViewModels;

namespace TaskFlow.Interfaces
{
    public interface IProjectService
    {
        Task<List<ProjectViewModel>> GetUserProjectsAsync(string userId);
        Task<ProjectViewModel?> GetProjectByIdAsync(int id, string userId);
        Task<Project> CreateProjectAsync(ProjectViewModel model, string userId);
        Task<bool> UpdateProjectAsync(ProjectViewModel model, string userId);
        Task<bool> DeleteProjectAsync(int id, string userId);
        Task<bool> IsProjectMemberAsync(int projectId, string userId);
    }

    public interface ITaskService
    {
        Task<List<TaskViewModel>> GetProjectTasksAsync(int projectId);
        Task<TaskDetailViewModel?> GetTaskDetailAsync(int taskId, string userId);
        Task<List<TaskViewModel>> GetUserTasksAsync(string userId);
        Task<ProjectTask> CreateTaskAsync(TaskViewModel model, string userId);
        Task<bool> UpdateTaskAsync(TaskViewModel model, string userId);
        Task<bool> UpdateTaskStatusAsync(int taskId, Models.TaskStatus status, string userId);
        Task<bool> DeleteTaskAsync(int id, string userId);
        Task<Comment> AddCommentAsync(int taskId, string content, string userId);
    }

    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardDataAsync(string userId);
    }
}
