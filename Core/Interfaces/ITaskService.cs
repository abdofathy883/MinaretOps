using Core.DTOs.Tasks;
using Core.Enums;

namespace Core.Interfaces
{
    public interface ITaskService
    {
        Task<List<TaskDTO>> GetAllTasksAsync();
        Task<List<TaskDTO>> GetTasksByEmployeeIdAsync(string empId);
        Task<TaskDTO> GetTaskByIdAsync(int taskId);
        Task<bool> ChangeTaskStatusAsync(int taskId, CustomTaskStatus status);
        Task<bool> UpdateTaskAsync(int taskId, UpdateTaskDTO updateTask);
        Task<bool> DeleteTaskAsync(int taskId);

        Task<TaskDTO> CreateTaskAsync(CreateTaskDTO createTask);
        Task<TaskGroupDTO> CreateTaskGroupAsync(CreateTaskGroupDTO createTaskGroup);
        Task<List<TaskGroupDTO>> GetTaskGroupsByClientServiceAsync(int clientServiceId);
    }
}
