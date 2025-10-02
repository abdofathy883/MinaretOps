using Core.DTOs.InternalTasks;
using Core.Enums;

namespace Core.Interfaces
{
    public interface IInternalTaskService
    {
        Task<List<InternalTaskDTO>> GetAllUnArchivedInternalTasksAsync();
        Task<List<InternalTaskDTO>> GetAllArchivedInternalTasksAsync();
        Task<List<InternalTaskDTO>> GetInternalTasksByEmpAsync(string empId);
        Task<InternalTaskDTO> GetInternalTaskById(int taskId);
        Task<InternalTaskDTO> CreateInternalTaskAsync(CreateInternalTaskDTO internalTaskDTO);
        Task<bool> ChangeTaskStatusAsync(int taskId, CustomTaskStatus status);
        Task<bool> ToggleArchiveInternalTaskAsync(int taskId);
        Task<InternalTaskDTO> UpdateInternalTaskAsync(int taskId, UpdateInternalTaskDTO internalTaskDTO);
        Task<bool> DeleteInternalTaskAsync(int taskId);
        Task<List<InternalTaskDTO>> SearchByTitleAsync(string query, string empId);

    }
}
