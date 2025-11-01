using Core.DTOs.Tasks;
using Core.DTOs.Tasks.CommentDTOs;
using Core.DTOs.Tasks.TaskDTOs;
using Core.DTOs.Tasks.TaskGroupDTOs;
using Core.DTOs.Tasks.TaskResourcesDTOs;
using Core.Enums;

namespace Core.Interfaces
{
    public interface ITaskService
    {
        Task<List<TaskDTO>> GetAllArchivedTasksAsync();
        Task<List<LightWieghtTaskDTO>> GetTasksByEmployeeIdAsync(string empId);
        Task<PaginatedTaskResultDTO> GetPaginatedTasksAsync(TaskFilterDTO filter, string currentUserId);
        Task<TaskDTO> GetTaskByIdAsync(int taskId);
        Task<bool> ChangeTaskStatusAsync(int taskId, CustomTaskStatus status, string userId);
        Task<TaskDTO> CompleteTaskAsync(int taskId, CreateTaskResourcesDTO taskResourcesDTO, string userId);
        Task<TaskDTO> UpdateTaskAsync(int taskId, UpdateTaskDTO updateTask, string userId);
        Task<bool> DeleteTaskAsync(int taskId);
        Task<TaskDTO> CreateTaskAsync(string userId, CreateTaskDTO createTask);
        Task<TaskGroupDTO> CreateTaskGroupAsync(CreateTaskGroupDTO createTaskGroup, string userId);
        Task<List<TaskGroupDTO>> GetTaskGroupsByClientServiceAsync(int clientServiceId);
        Task<List<LightWieghtTaskDTO>> SearchTasks(string query, string currentUserId);
        Task<TaskCommentDTO> AddCommentAsync(CreateTaskCommentDTO taskComment);
    }
}
