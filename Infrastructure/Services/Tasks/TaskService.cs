using AutoMapper;
using Core.DTOs.Tasks;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.NewFolder
{
    public class TaskService : ITaskService
    {
        private readonly MinaretOpsDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;
        public TaskService(
            MinaretOpsDbContext minaret,
            IMapper _mapper,
            UserManager<ApplicationUser> manager
            )
        {
            context = minaret;
            mapper = _mapper;
            userManager = manager;
        }
        public async Task<bool> ChangeTaskStatusAsync(int taskId, CustomTaskStatus status)
        {
            var task = await GetTaskOrThrow(taskId);

            task.Status = status;

            context.Update(task);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task DeleteTaskAsync(int taskId)
        {
            var task = await GetTaskOrThrow(taskId);

            context.Remove(task);
            await context.SaveChangesAsync();
        }

        public async Task<List<TaskDTO>> GetAllTasksAsync()
        {
            var tasks = await context.Tasks
                .Include(t => t.ClientService)
                    .ThenInclude(cs => cs.Service)
                .Include(t => t.ClientService)
                    .ThenInclude(cs => cs.Client)
                .Include(t => t.Employee)
                .ToListAsync();

            return mapper.Map<List<TaskDTO>>(tasks);
        }

        public async Task<TaskDTO> GetTaskByIdAsync(int taskId)
        {
            var task = await context.Tasks
                .Include(t => t.ClientService)
                    .ThenInclude(cs => cs.Service)
                .Include(t => t.ClientService)
                    .ThenInclude(cs => cs.Client)
                .Include(t => t.Employee)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            return mapper.Map<TaskDTO>(task);
        }

        public async Task<List<TaskDTO>> GetTasksByEmployeeIdAsync(string empId)
        {
            var tasks = await context.Tasks
                .Include(t => t.ClientService)
                    .ThenInclude(cs => cs.Service)
                .Include(t => t.ClientService)
                    .ThenInclude(cs => cs.Client)
                .Include(t => t.Employee)
                .Where(t => t.EmployeeId == empId)
                .ToListAsync();

            return mapper.Map<List<TaskDTO>>(tasks);
        }

        public async Task<bool> UpdateTaskAsync(int taskId, UpdateTaskDTO updateTask)
        {
            if (updateTask is null)
                throw new Exception();
            var task = await GetTaskOrThrow(taskId);

            task.Title = updateTask.Title ?? task.Title;
            task.Description = updateTask.Description ?? task.Description;
            task.Deadline = updateTask.Deadline ?? task.Deadline;
            task.Priority = updateTask.Priority ?? task.Priority;
            task.Refrence = updateTask.Refrence ?? task.Refrence;
            task.Status = updateTask.Status;

            if (!string.IsNullOrWhiteSpace(updateTask.EmployeeId) && 
                updateTask.EmployeeId != task.EmployeeId)
            {
                var user = await userManager.FindByIdAsync(updateTask.EmployeeId)
                    ?? throw new Exception();
                task.EmployeeId = user.Id;
            }

            context.Update(task);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<TaskDTO> CreateTaskAsync(CreateTaskDTO createTask)
        {
            // Validate that the task group exists
            var taskGroup = await context.TaskGroups
                .FirstOrDefaultAsync(tg => tg.Id == createTask.TaskGroupId)
                ?? throw new Exception("Task group not found");

            // Validate that the client service exists
            var clientService = await context.ClientServices
                .FirstOrDefaultAsync(cs => cs.Id == createTask.ClientServiceId)
                ?? throw new Exception("Client service not found");

            // Validate that the employee exists
            var employee = await userManager.FindByIdAsync(createTask.EmployeeId)
                ?? throw new Exception("Employee not found");

            var task = new TaskItem
            {
                Title = createTask.Title,
                Description = createTask.Description,
                Status = createTask.Status,
                ClientServiceId = createTask.ClientServiceId,
                Deadline = createTask.Deadline,
                Priority = createTask.Priority,
                Refrence = createTask.Refrence,
                EmployeeId = createTask.EmployeeId,
                TaskGroupId = createTask.TaskGroupId
            };

            context.Tasks.Add(task);
            await context.SaveChangesAsync();

            // Return the created task with all related data
            var createdTask = await context.Tasks
                .Include(t => t.ClientService)
                    .ThenInclude(cs => cs.Service)
                .Include(t => t.ClientService)
                    .ThenInclude(cs => cs.Client)
                .Include(t => t.Employee)
                .Include(t => t.TaskGroup)
                .FirstOrDefaultAsync(t => t.Id == task.Id);

            return mapper.Map<TaskDTO>(createdTask);
        }

        public async Task<TaskGroupDTO> CreateTaskGroupAsync(CreateTaskGroupDTO createTaskGroup)
        {
            // Validate that the client service exists
            var clientService = await context.ClientServices
                .FirstOrDefaultAsync(cs => cs.Id == createTaskGroup.ClientServiceId)
                ?? throw new Exception("Client service not found");

            // Check if task group already exists for this month/year
            var existingGroup = await context.TaskGroups
                .FirstOrDefaultAsync(tg => tg.ClientServiceId == createTaskGroup.ClientServiceId
                                          && tg.Month == createTaskGroup.Month
                                          && tg.Year == createTaskGroup.Year);

            if (existingGroup != null)
                throw new Exception("Task group already exists for this month and year");

            var taskGroup = new TaskGroup
            {
                ClientServiceId = createTaskGroup.ClientServiceId,
                Month = createTaskGroup.Month,
                Year = createTaskGroup.Year,
                MonthLabel = createTaskGroup.MonthLabel
            };

            context.TaskGroups.Add(taskGroup);
            await context.SaveChangesAsync();

            // Create tasks if provided
            if (createTaskGroup.Tasks.Any())
            {
                foreach (var taskDto in createTaskGroup.Tasks)
                {
                    var employee = await userManager.FindByIdAsync(taskDto.EmployeeId)
                        ?? throw new Exception($"Employee with ID {taskDto.EmployeeId} not found");

                    var task = new TaskItem
                    {
                        Title = taskDto.Title,
                        Description = taskDto.Description,
                        Status = taskDto.Status,
                        ClientServiceId = taskDto.ClientServiceId,
                        Deadline = taskDto.Deadline,
                        Priority = taskDto.Priority,
                        Refrence = taskDto.Refrence,
                        EmployeeId = taskDto.EmployeeId,
                        TaskGroupId = taskGroup.Id
                    };

                    context.Tasks.Add(task);
                }
                await context.SaveChangesAsync();
            }

            // Return the created task group with all related data
            var createdTaskGroup = await context.TaskGroups
                .Include(tg => tg.Tasks)
                    .ThenInclude(t => t.Employee)
                .Include(tg => tg.Tasks)
                    .ThenInclude(t => t.ClientService)
                        .ThenInclude(cs => cs.Service)
                .Include(tg => tg.Tasks)
                    .ThenInclude(t => t.ClientService)
                        .ThenInclude(cs => cs.Client)
                .FirstOrDefaultAsync(tg => tg.Id == taskGroup.Id);

            return mapper.Map<TaskGroupDTO>(createdTaskGroup);
        }

        public async Task<List<TaskGroupDTO>> GetTaskGroupsByClientServiceAsync(int clientServiceId)
        {
            var taskGroups = await context.TaskGroups
                .Include(tg => tg.Tasks)
                    .ThenInclude(t => t.Employee)
                .Include(tg => tg.Tasks)
                    .ThenInclude(t => t.ClientService)
                        .ThenInclude(cs => cs.Service)
                .Include(tg => tg.Tasks)
                    .ThenInclude(t => t.ClientService)
                        .ThenInclude(cs => cs.Client)
                .Where(tg => tg.ClientServiceId == clientServiceId)
                .OrderByDescending(tg => tg.Year)
                .ThenByDescending(tg => tg.Month)
                .ToListAsync();

            return mapper.Map<List<TaskGroupDTO>>(taskGroups);
        }
        private async Task<TaskItem> GetTaskOrThrow(int taskId)
        {
            var task = await context.Tasks
                .FirstOrDefaultAsync(t => t.Id == taskId)
                ?? throw new Exception();

            return task;
        }
    }
}
