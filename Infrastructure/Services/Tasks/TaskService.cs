using AutoMapper;
using Core.DTOs.Tasks;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Infrastructure.Services.Discord;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.NewFolder
{
    public class TaskService : ITaskService
    {
        private readonly MinaretOpsDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEmailService emailService;
        private readonly INotificationService notificationService;
        private readonly DiscordService discordService;
        public TaskService(
            MinaretOpsDbContext minaret,
            IMapper _mapper,
            UserManager<ApplicationUser> manager,
            IEmailService email,
            INotificationService _notificationService,
            DiscordService _discordService
            )
        {
            context = minaret;
            mapper = _mapper;
            userManager = manager;
            emailService = email;
            notificationService = _notificationService;
            discordService = _discordService;
        }
        public async Task<bool> ChangeTaskStatusAsync(int taskId, CustomTaskStatus status)
        {
            var task = await GetTaskOrThrow(taskId);

            task.Status = status;
            if (status == CustomTaskStatus.Completed)
            {
                task.CompletedAt = DateTime.UtcNow;
            }
            context.Update(task);
            await context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(task.Employee.Email) && !string.IsNullOrEmpty(task.EmployeeId))
            {
                Dictionary<string, string> replacements = new Dictionary<string, string>
                {
                    {"FullName", $"{task.Employee.FirstName} {task.Employee.LastName}" },
                    {"Email", $"{task.Employee.Email}" },
                    {"TaskTitle", $"{task.Title}" },
                    {"TaskType", $"{task.TaskType}" },
                    {"TaskId", $"{task.Id}" },
                    {"OldStatus", $"{task.Status}" },
                    {"NewStatus", $"{status}" },
                    {"TimeStamp", $"{DateTime.UtcNow}" }
                };
                await emailService.SendEmailWithTemplateAsync(task.Employee.Email, "Task Updates", "ChangeTaskStatus", replacements);
                //await notificationService.CreateAsync(new Core.DTOs.Notifications.CreateNotificationDTO
                //{
                //    UserId = task.EmployeeId,
                //    Title = "Task Status Updated",
                //    Body = $"The status of task '{task.Title}' has been changed to {status}.",
                //    Url = $"https://internal.theminaretagency.com/tasks/{task.Id}"
                //});
                
            }

            string channel = task.ClientService.Client.DiscordChannelId;
            if (!string.IsNullOrEmpty(channel))
            {
                TaskDTO mappedTask = mapper.Map<TaskDTO>(task);
                await discordService.ChangeTaskStatus(channel, mappedTask, status);
            }
            return true;
        }
        public async Task<bool> DeleteTaskAsync(int taskId)
        {
            var task = await GetTaskOrThrow(taskId);

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                context.Remove(task);
                await context.SaveChangesAsync();

                if (!string.IsNullOrEmpty(task.Employee.Email) && !string.IsNullOrEmpty(task.EmployeeId))
                {
                    Dictionary<string, string> replacements = new Dictionary<string, string>
                    {
                        {"FullName", $"{task.Employee.FirstName} {task.Employee.LastName}" },
                        {"Email", $"{task.Employee.Email}" },
                        {"TaskTitle", $"{task.Title}" },
                        {"TaskType", $"{task.TaskType}" },
                        {"TaskId", $"{task.Id}" },
                        {"TimeStamp", $"{DateTime.UtcNow}" }
                    };
                    await emailService.SendEmailWithTemplateAsync(task.Employee.Email, "Task Updates", "ChangeTaskStatus", replacements);
                }
                string channel = task.ClientService.Client.DiscordChannelId;
                if (!string.IsNullOrEmpty(channel))
                {
                    TaskDTO mappedTask = mapper.Map<TaskDTO>(task);
                    await discordService.DeleteTask(channel, mappedTask);
                }
                await transaction.CommitAsync();
                return true;
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception();
            }

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
        public async Task<TaskDTO> UpdateTaskAsync(int taskId, UpdateTaskDTO updateTask)
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
            await context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(task.Employee.Email) && !string.IsNullOrEmpty(task.EmployeeId))
            {
                Dictionary<string, string> replacements = new Dictionary<string, string>
                {
                    {"FullName", $"{task.Employee.FirstName} {task.Employee.LastName}" },
                    {"Email", $"{task.Employee.Email}" },
                    {"TaskTitle", $"{task.Title}" },
                    {"TaskType", $"{task.TaskType}" },
                    {"TaskId", $"{task.Id}" },
                    {"TimeStamp", $"{DateTime.UtcNow}" }
                };
                await emailService.SendEmailWithTemplateAsync(task.Employee.Email, "Task Has Been Updated", "TaskUpdates", replacements);
            }

            string channel = task.ClientService.Client.DiscordChannelId ?? string.Empty;
            TaskDTO mappedTask = mapper.Map<TaskDTO>(task);
            if (!string.IsNullOrEmpty(channel))
            {
                await discordService.UpdateTask(channel, mappedTask);    
            }

            return mappedTask;
        }
        public async Task<TaskDTO> CreateTaskAsync(CreateTaskDTO createTask)
        {
            // Validate that the task group exists
            var taskGroup = await context.TaskGroups
                .FirstOrDefaultAsync(tg => tg.Id == createTask.TaskGroupId)
                ?? throw new InvalidObjectException("لم نتمكن من العثور على مجموعة التاسكات");

            // Validate that the client service exists
            var clientService = await context.ClientServices
                .Include(cs => cs.Client)
                .FirstOrDefaultAsync(cs => cs.Id == createTask.ClientServiceId)
                ?? throw new InvalidObjectException("العميل غير مشترك في هذه الخدمة");

            // Validate that the employee exists
            var employee = await userManager.FindByIdAsync(createTask.EmployeeId)
                ?? throw new InvalidObjectException("لم نتمكن من العثور على الموظف");

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var task = new TaskItem
                {
                    Title = createTask.Title,
                    TaskType = createTask.TaskType,
                    Description = createTask.Description,
                    Status = CustomTaskStatus.Open,
                    ClientServiceId = createTask.ClientServiceId,
                    Deadline = createTask.Deadline,
                    Priority = createTask.Priority,
                    Refrence = createTask.Refrence,
                    EmployeeId = createTask.EmployeeId,
                    TaskGroupId = createTask.TaskGroupId
                };

                await context.Tasks.AddAsync(task);
                await context.SaveChangesAsync();

                if (!string.IsNullOrEmpty(task.Employee.Email) && !string.IsNullOrEmpty(task.EmployeeId))
                {
                    Dictionary<string, string> replacements = new Dictionary<string, string>
                    {
                        {"FullName", $"{task.Employee.FirstName} {task.Employee.LastName}" },
                        {"Email", $"{task.Employee.Email}" },
                        {"TaskTitle", $"{task.Title}" },
                        {"TaskType", $"{task.TaskType}" },
                        {"TaskId", $"{task.Id}" },
                        {"TimeStamp", $"{DateTime.UtcNow}" }
                    };
                    await emailService.SendEmailWithTemplateAsync(task.Employee.Email, "New Task Has Been Assigned To You", "NewTaskAssignment", replacements);
                }
                string channel = task.ClientService.Client.DiscordChannelId;
                if (!string.IsNullOrEmpty(channel))
                {
                    TaskDTO mappedTask = mapper.Map<TaskDTO>(task);
                    await discordService.NewTask(channel, mappedTask);
                }


                // Return the created task with all related data
                var createdTask = await context.Tasks
                    .Include(t => t.ClientService)
                        .ThenInclude(cs => cs.Service)
                    .Include(t => t.ClientService)
                        .ThenInclude(cs => cs.Client)
                    .Include(t => t.Employee)
                    .Include(t => t.TaskGroup)
                    .FirstOrDefaultAsync(t => t.Id == task.Id);

                await transaction.CommitAsync();

                return mapper.Map<TaskDTO>(createdTask);

            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }
        public async Task<TaskGroupDTO> CreateTaskGroupAsync(CreateTaskGroupDTO createTaskGroup)
        {
            // Validate that the client service exists
            var clientService = await context.ClientServices
                .Include(cs => cs.Client)
                .Include(cs => cs.Service)
                .FirstOrDefaultAsync(cs => cs.Id == createTaskGroup.ClientServiceId)
                ?? throw new InvalidObjectException("العميل غير مشترك في هذه الخدمة");

            // Check if task group already exists for this month/year
            var existingGroup = await context.TaskGroups
                .FirstOrDefaultAsync(tg => tg.ClientServiceId == createTaskGroup.ClientServiceId 
                && tg.Month == DateTime.Now.Month 
                && tg.Year == DateTime.Now.Year);

            if (existingGroup != null)
                throw new AlreadyExistObjectException("مجموعة التاسكات لهذا الشهر موجودة بالفعل");

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var taskGroup = new TaskGroup
                {
                    ClientServiceId = createTaskGroup.ClientServiceId,
                    Month = DateTime.Now.Month,
                    Year = DateTime.Now.Year,
                    MonthLabel = $"{DateTime.Now.ToString("MMMM")} {DateTime.Now.ToString("yyyy")}"
                };

                context.TaskGroups.Add(taskGroup);
                await context.SaveChangesAsync();

                // Create tasks if provided
                if (createTaskGroup.Tasks.Any())
                {
                    foreach (var taskDto in createTaskGroup.Tasks)
                    {
                        var employee = await userManager.FindByIdAsync(taskDto.EmployeeId)
                            ?? throw new InvalidObjectException($"Employee with ID {taskDto.EmployeeId} not found");

                        var task = new TaskItem
                        {
                            Title = taskDto.Title,
                            TaskType = taskDto.TaskType,
                            Description = taskDto.Description,
                            Status = taskDto.Status,
                            ClientServiceId = createTaskGroup.ClientServiceId,
                            Deadline = taskDto.Deadline,
                            Priority = taskDto.Priority,
                            Refrence = taskDto.Refrence,
                            EmployeeId = taskDto.EmployeeId,
                            TaskGroupId = taskGroup.Id
                        };

                        context.Tasks.Add(task);
                        await context.SaveChangesAsync();

                        if (!string.IsNullOrEmpty(task.Employee.Email) && !string.IsNullOrEmpty(task.EmployeeId))
                        {
                            Dictionary<string, string> replacements = new Dictionary<string, string>
                            {
                                {"FullName", $"{task.Employee.FirstName} {task.Employee.LastName}" },
                                {"Email", $"{task.Employee.Email}" },
                                {"TaskTitle", $"{task.Title}" },
                                {"TaskType", $"{task.TaskType}" },
                                {"TaskId", $"{task.Id}" },
                                {"Client", $"{task.ClientService.Client.Name}" },
                                {"TimeStamp", $"{DateTime.UtcNow}" }
                            };
                            await emailService.SendEmailWithTemplateAsync(task.Employee.Email, "New Task Has been Assigned To You", "NewTaskAssignment", replacements);
                        }

                        string channel = task.ClientService.Client.DiscordChannelId ?? string.Empty;
                        TaskDTO mappedTask = mapper.Map<TaskDTO>(task);

                        await discordService.NewTask(channel, mappedTask);                        
                    }
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

                await transaction.CommitAsync();
                return mapper.Map<TaskGroupDTO>(createdTaskGroup);
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
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
                .Include(e => e.Employee)
                .FirstOrDefaultAsync(t => t.Id == taskId)
                ?? throw new InvalidObjectException("لا يوجد تاسك بهذه البيانات");

            return task;
        }
    }
}
