using AutoMapper;
using Core.DTOs.Tasks.TaskDTOs;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Tasks
{
    public class ArchivedTaskService : IArchivedTaskService
    {
        private readonly MinaretOpsDbContext context;
        private readonly TaskHelperService helperService;
        private readonly IMapper mapper;

        public ArchivedTaskService(MinaretOpsDbContext context, TaskHelperService helperService, IMapper mapper)
        {
            this.context = context;
            this.helperService = helperService;
            this.mapper = mapper;
        }

        public async Task<TaskDTO> ArchiveTaskAsync(int taskId)
        {
            var task = await helperService.GetTaskOrThrow(taskId);

            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                // Load related data
                await context.Entry(task)
                    .Collection(t => t.TaskHistory)
                    .LoadAsync();
                
                await context.Entry(task)
                    .Collection(t => t.CompletionResources)
                    .LoadAsync();

                // Create ArchivedTask from TaskItem
                var archivedTask = new ArchivedTask
                {
                    Title = task.Title,
                    TaskType = task.TaskType,
                    Description = task.Description,
                    Status = task.Status,
                    ClientServiceId = task.ClientServiceId,
                    Deadline = task.Deadline,
                    Priority = task.Priority,
                    Refrence = task.Refrence,
                    EmployeeId = task.EmployeeId,
                    CompletedAt = task.CompletedAt,
                    TaskGroupId = task.TaskGroupId,
                    CompletionNotes = task.CompletionNotes,
                    CreatedAt = task.CreatedAt
                };

                // Add to archived tasks first (so we get the ID)
                context.ArchivedTasks.Add(archivedTask);
                await context.SaveChangesAsync(); // Save to get the archived task ID

                // Update TaskHistory: Set ArchivedTaskId and clear TaskItemId
                foreach (var history in task.TaskHistory)
                {
                    history.ArchivedTaskId = archivedTask.Id;
                    history.TaskItemId = null;
                    context.TaskHistory.Update(history);
                }

                // Update TaskCompletionResources: Set ArchivedTaskId and clear TaskId
                foreach (var resource in task.CompletionResources)
                {
                    resource.ArchivedTaskId = archivedTask.Id;
                    resource.TaskId = null;
                    context.TaskCompletionResources.Update(resource);
                }

                // Remove from active tasks
                context.Tasks.Remove(task);

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return mapper.Map<TaskDTO>(archivedTask);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<TaskDTO> RestoreTaskAsync(int taskId)
        {
            var archivedTask = await context.ArchivedTasks
                .Include(t => t.TaskHistory)
                .Include(t => t.CompletionResources)
                .FirstOrDefaultAsync(t => t.Id == taskId)
                ?? throw new InvalidObjectException("لم يتم العثور على التاسك المؤرشف");

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var task = new TaskItem
                {
                    Title = archivedTask.Title,
                    TaskType = archivedTask.TaskType,
                    Description = archivedTask.Description,
                    Status = archivedTask.Status,
                    ClientServiceId = archivedTask.ClientServiceId,
                    Deadline = archivedTask.Deadline,
                    Priority = archivedTask.Priority,
                    Refrence = archivedTask.Refrence,
                    EmployeeId = archivedTask.EmployeeId,
                    CompletedAt = archivedTask.CompletedAt,
                    TaskGroupId = archivedTask.TaskGroupId,
                    CompletionNotes = archivedTask.CompletionNotes,
                    CreatedAt = archivedTask.CreatedAt,
                };

                // Add to active tasks first (so we get the ID)
                context.Tasks.Add(task);
                await context.SaveChangesAsync(); // Save to get the task ID

                // Update TaskHistory: Set TaskItemId and clear ArchivedTaskId
                foreach (var history in archivedTask.TaskHistory)
                {
                    history.TaskItemId = task.Id;
                    history.ArchivedTaskId = null;
                    context.TaskHistory.Update(history);
                }

                // Update TaskCompletionResources: Set TaskId and clear ArchivedTaskId
                foreach (var resource in archivedTask.CompletionResources)
                {
                    resource.TaskId = task.Id;
                    resource.ArchivedTaskId = null;
                    context.TaskCompletionResources.Update(resource);
                }

                // Remove from archived tasks
                context.ArchivedTasks.Remove(archivedTask);

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return mapper.Map<TaskDTO>(task);
            }
            catch(Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<TaskDTO> GetArchivedTaskByIdAsync(int taskId)
        {
            var task = await context.ArchivedTasks
                .Include(t => t.CompletionResources)
                .Include(t => t.ClientService)
                    .ThenInclude(cs => cs.Service)
                .Include(t => t.ClientService)
                    .ThenInclude(cs => cs.Client)
                .Include(t => t.Employee)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null)
                throw new Exception();

            await context.Entry(task)
                .Collection(t => t.TaskHistory)
                .Query()
                .Include(th => th.UpdatedBy)
                .LoadAsync();

            await context.Entry(task)
                .Collection(t => t.CompletionResources)
                .LoadAsync();

            //await context.Entry(task)
            //    .Collection(t => t.TaskComments)
            //    .Query()
            //    .Include(t => t.Employee)
            //    .LoadAsync();

            return mapper.Map<TaskDTO>(task);
        }
    }
}
