using Application.DTOs.Tasks.TaskDTOs;

namespace Application.Interfaces
{
    public interface IArchivedTaskService
    {
        Task<TaskDTO> ArchiveTaskAsync(int taskId);
        Task<TaskDTO> RestoreTaskAsync(int taskId);
        Task<TaskDTO> GetArchivedTaskByIdAsync(int taskId);
    }
}
