using Core.DTOs.Tasks.TaskDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IArchivedTaskService
    {
        Task<TaskDTO> ArchiveTaskAsync(int taskId);
        Task<TaskDTO> RestoreTaskAsync(int taskId);
        Task<TaskDTO> GetArchivedTaskByIdAsync(int taskId);
    }
}
