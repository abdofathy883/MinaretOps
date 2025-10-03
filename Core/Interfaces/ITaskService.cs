using Core.DTOs.Tasks;
using Core.Enums;

namespace Core.Interfaces
{
    public interface ITaskService
    {
        Task<List<TaskDTO>> GetAllArchivedTasksAsync();
        Task<List<TaskDTO>> GetTasksByEmployeeIdAsync(string empId);
        Task<TaskDTO> GetTaskByIdAsync(int taskId);
        Task<bool> ChangeTaskStatusAsync(int taskId, CustomTaskStatus status, string userId);
        Task<TaskDTO> CompleteTaskAsync(int taskId, CreateTaskResourcesDTO taskResourcesDTO, string userId);
        Task<TaskDTO> ToggleArchiveTaskAsync(int taskId);
        Task<TaskDTO> UpdateTaskAsync(int taskId, UpdateTaskDTO updateTask, string userId);
        Task<bool> DeleteTaskAsync(int taskId);

        Task<TaskDTO> CreateTaskAsync(CreateTaskDTO createTask);
        Task<TaskGroupDTO> CreateTaskGroupAsync(CreateTaskGroupDTO createTaskGroup);
        Task<List<TaskGroupDTO>> GetTaskGroupsByClientServiceAsync(int clientServiceId);
        Task<List<TaskDTO>> SearchTasks(string query, string currentUserId);
    }
}
