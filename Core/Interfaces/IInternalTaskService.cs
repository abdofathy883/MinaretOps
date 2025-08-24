using Core.DTOs.InternalTasks;
using Core.Enums;

namespace Core.Interfaces
{
    public interface IInternalTaskService
    {
        Task<List<InternalTaskDTO>> GetAllInternalTasksAsync();
        Task<InternalTaskDTO> GetInternalTaskById(int taskId);
        Task<InternalTaskDTO> CreateInternalTaskAsync(CreateInternalTaskDTO internalTaskDTO);
        Task<bool> ChangeTaskStatusAsync(int taskId, CustomTaskStatus status);

    }
}
