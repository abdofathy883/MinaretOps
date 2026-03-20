using Application.DTOs.Tasks;
using Application.DTOs.Tasks.CommentDTOs;
using Application.DTOs.Tasks.TaskDTOs;
using Application.DTOs.Tasks.TaskGroupDTOs;
using Application.DTOs.Tasks.TaskResourcesDTOs;
using Core.Enums;

namespace Application.Interfaces
{
    public interface ITaskService
    {
        Task<List<TaskDTO>> GetAllArchivedTasksAsync();
        Task<List<LightWieghtTaskDTO>> GetAllCompletedAsync();
        Task<List<LightWieghtTaskDTO>> GetTasksByEmployeeIdAsync(string empId);
        Task<PaginatedTaskResultDTO> GetPaginatedTasksAsync(TaskFilterDTO filter, string currentUserId);
        Task<TaskDTO> GetTaskByIdAsync(int taskId);
        Task<bool> ChangeTaskStatusAsync(int taskId, CustomTaskStatus status, string userId);
        Task<TaskDTO> CompleteTaskAsync(int taskId, CreateTaskResourcesDTO taskResourcesDTO, string userId);
        Task<TaskDTO> UpdateTaskAsync(UpdateTaskDTO updateTask, string userId);
        Task<bool> DeleteTaskAsync(int taskId);
        Task<TaskDTO> CreateTaskAsync(string userId, CreateTaskDTO createTask);
        Task<TaskGroupDTO> CreateTaskGroupAsync(CreateTaskGroupDTO createTaskGroup, string userId);
        Task<List<TaskGroupDTO>> GetTaskGroupsByClientServiceAsync(int clientServiceId);
        Task<List<LightWieghtTaskDTO>> SearchTasks(string query, string currentUserId);
        Task<TaskCommentDTO> AddCommentAsync(CreateTaskCommentDTO taskComment);
    }
}
