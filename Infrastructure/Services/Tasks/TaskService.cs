using AutoMapper;
using Core.DTOs.Tasks;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Infrastructure.Services.Discord;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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
        public async Task<bool> ChangeTaskStatusAsync(int taskId, CustomTaskStatus status, string userId)
        {
            if (status == CustomTaskStatus.Completed)
                throw new InvalidOperationException("لا يمكن انهاء التاسك من خلال هذا الجراء");

            var user = await userManager.FindByIdAsync(userId);

            var task = await GetTaskOrThrow(taskId);
            var oldStatus = task.Status;

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                task.Status = status;
                var history = new TaskItemHistory
                {
                    TaskItemId = taskId,
                    PropertyName = "تغيير الحالة",
                    OldValue = oldStatus.ToString(),
                    NewValue = status.ToString(),
                    UpdatedById = userId,
                    UpdatedAt = DateTime.UtcNow
                };
                context.Tasks.Update(task);
                await context.TaskHistory.AddAsync(history);
                await context.SaveChangesAsync();
                if (user is not null)
                {
                    if (!string.IsNullOrEmpty(user.Email) && !string.IsNullOrEmpty(user.Id))
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
                        await emailService.SendEmailWithTemplateAsync(user.Email, "Task Updates", "ChangeTaskStatus", replacements);
                        //await notificationService.CreateAsync(new Core.DTOs.Notifications.CreateNotificationDTO
                        //{
                        //    UserId = task.EmployeeId,
                        //    Title = "Task Status Updated",
                        //    Body = $"The status of task '{task.Title}' has been changed to {status}.",
                        //    Url = $"https://internal.theminaretagency.com/tasks/{task.Id}"
                        //});
                
                    }
                    
                }
                string? channel = task.ClientService?.Client?.DiscordChannelId;
                if (!string.IsNullOrEmpty(channel))
                {
                    TaskDTO mappedTask = mapper.Map<TaskDTO>(task);
                    await discordService.ChangeTaskStatus(channel, mappedTask, status);
                }
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
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
                    await emailService.SendEmailWithTemplateAsync(task.Employee.Email, "Task Deleted", "DeleteTask", replacements);
                }
                string? channel = task.ClientService?.Client?.DiscordChannelId;
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
                throw new Exception(ex.Message);
            }

        }
        public async Task<List<TaskDTO>> GetAllArchivedTasksAsync()
        {
            var tasks = await context.Tasks
                .Where(t => t.IsArchived)
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
                .Include(t => t.TaskHistory)
                .Include(t => t.CompletionResources)
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
            var emp = await userManager.FindByIdAsync(empId)
                ?? throw new Exception("لم يتم العثور على الموظف");

            var roles = await userManager.GetRolesAsync(emp);

            IQueryable<TaskItem> query = context.Tasks
                .Where(t => !t.IsArchived)
                .Include(t => t.ClientService)
                    .ThenInclude(cs => cs.Service)
                .Include(t => t.ClientService)
                    .ThenInclude(cs => cs.Client)
                .Include(t => t.Employee);

            if (roles.Contains(UserRoles.Admin.ToString()) || roles.Contains(UserRoles.AccountManager.ToString()))
            {
                // Return all tasks (no empId filter)
            }
            else if (roles.Contains("ContentCreatorTeamLeader"))
            {
                var contentTypes = new[] { TaskType.ContentWriting, TaskType.ContentStrategy };
                query = query.Where(t => contentTypes.Contains(t.TaskType));
            }
            else if (roles.Contains("GraphicDesignerTeamLeader"))
            {
                var contentTypes = new[]
                {
                    TaskType.Illustrations,
                    TaskType.LogoDesign,
                    TaskType.VisualIdentity,
                    TaskType.DesignDirections,
                    TaskType.SM_Design,
                    TaskType.Motion,
                    TaskType.VideoEditing
                };
                query = query.Where(t => contentTypes.Contains(t.TaskType));
            }
            else
            {
                // Default: return only tasks assigned to this employee
                query = query.Where(t => t.EmployeeId == empId);
            }

            var tasks = await query.ToListAsync();
            return mapper.Map<List<TaskDTO>>(tasks);
        }
        public async Task<TaskDTO> UpdateTaskAsync(int taskId, UpdateTaskDTO updateTask, string userId)
        {
            var task = await GetTaskOrThrow(taskId);
            var histories = new List<TaskItemHistory>();

            // --- Title ---
            if (!string.IsNullOrWhiteSpace(updateTask.Title) && updateTask.Title != task.Title)
            {
                histories.Add(new TaskItemHistory
                {
                    TaskItemId = task.Id,
                    PropertyName = "العنوان",
                    OldValue = task.Title,
                    NewValue = updateTask.Title,
                    UpdatedById = userId,
                    UpdatedAt = DateTime.UtcNow
                });
                task.Title = updateTask.Title;
            }

            // --- Description ---
            if (!string.IsNullOrWhiteSpace(updateTask.Description) && updateTask.Description != task.Description)
            {
                histories.Add(new TaskItemHistory
                {
                    TaskItemId = task.Id,
                    PropertyName = "الوصف",
                    OldValue = task.Description,
                    NewValue = updateTask.Description,
                    UpdatedById = userId,
                    UpdatedAt = DateTime.UtcNow
                });
                task.Description = updateTask.Description;
            }

            // --- Deadline ---
            if (updateTask.Deadline.HasValue && updateTask.Deadline.Value != task.Deadline)
            {
                histories.Add(new TaskItemHistory
                {
                    TaskItemId = task.Id,
                    PropertyName = "الموعد النهائي",
                    OldValue = task.Deadline.ToString("u"),
                    NewValue = updateTask.Deadline.Value.ToString("u"),
                    UpdatedById = userId,
                    UpdatedAt = DateTime.UtcNow
                });
                task.Deadline = updateTask.Deadline.Value;
            }

            // --- Priority ---
            if (!string.IsNullOrWhiteSpace(updateTask.Priority) && updateTask.Priority != task.Priority)
            {
                histories.Add(new TaskItemHistory
                {
                    TaskItemId = task.Id,
                    PropertyName = "الأولوية",
                    OldValue = task.Priority,
                    NewValue = updateTask.Priority,
                    UpdatedById = userId,
                    UpdatedAt = DateTime.UtcNow
                });
                task.Priority = updateTask.Priority;
            }

            // --- Reference ---
            if (!string.IsNullOrWhiteSpace(updateTask.Refrence) && updateTask.Refrence != task.Refrence)
            {
                histories.Add(new TaskItemHistory
                {
                    TaskItemId = task.Id,
                    PropertyName = "المرجع",
                    OldValue = task.Refrence,
                    NewValue = updateTask.Refrence,
                    UpdatedById = userId,
                    UpdatedAt = DateTime.UtcNow
                });
                task.Refrence = updateTask.Refrence;
            }

            // --- Status ---
            if (updateTask.Status != task.Status)
            {
                histories.Add(new TaskItemHistory
                {
                    TaskItemId = task.Id,
                    PropertyName = "الحالة",
                    OldValue = task.Status.ToString(),
                    NewValue = updateTask.Status.ToString(),
                    UpdatedById = userId,
                    UpdatedAt = DateTime.UtcNow
                });
                task.Status = updateTask.Status;
            }

            // --- Employee ---
            if (!string.IsNullOrWhiteSpace(updateTask.EmployeeId) && updateTask.EmployeeId != task.EmployeeId)
            {
                var user = await userManager.FindByIdAsync(updateTask.EmployeeId)
                    ?? throw new Exception("Employee not found");

                histories.Add(new TaskItemHistory
                {
                    TaskItemId = task.Id,
                    PropertyName = "الموظف",
                    OldValue = task.EmployeeId ?? "غير محدد",
                    NewValue = user.Id,
                    UpdatedById = userId,
                    UpdatedAt = DateTime.UtcNow
                });

                task.EmployeeId = user.Id;
            }

            // --- Save task + history in one transaction ---
            context.Tasks.Update(task);
            if (histories.Any())
                await context.TaskHistory.AddRangeAsync(histories);

            await context.SaveChangesAsync();

            // --- Notifications ---
            if (!string.IsNullOrEmpty(task.Employee.Email) && !string.IsNullOrEmpty(task.EmployeeId))
            {
                var replacements = new Dictionary<string, string>
                {
                    {"FullName", $"{task.Employee.FirstName} {task.Employee.LastName}" },
                    {"Email", task.Employee.Email },
                    {"TaskTitle", task.Title },
                    {"TaskType", task.TaskType.ToString() },
                    {"TaskId", task.Id.ToString() },
                    {"TimeStamp", DateTime.UtcNow.ToString("u") }
                };

                await emailService.SendEmailWithTemplateAsync(
                    task.Employee.Email,
                    "Task Has Been Updated",
                    "TaskUpdates",
                    replacements
                );
            }

            // --- Discord update ---
            string? channel = task.ClientService?.Client?.DiscordChannelId;
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

            var emp = await userManager.FindByIdAsync(createTask.EmployeeId)
                ?? throw new Exception();

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
                    EmployeeId = createTask.EmployeeId ?? string.Empty,
                    TaskGroupId = createTask.TaskGroupId
                };

                await context.Tasks.AddAsync(task);
                await context.SaveChangesAsync();

                if (!string.IsNullOrEmpty(emp.Email) && !string.IsNullOrEmpty(task.EmployeeId))
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
                    await emailService.SendEmailWithTemplateAsync(emp.Email, "New Task Has Been Assigned To You", "NewTaskAssignment", replacements);
                }
                string? channel = task.ClientService?.Client?.DiscordChannelId;
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
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                // Validate that the client service exists
                var clientService = await context.ClientServices
                    .FirstOrDefaultAsync(cs => cs.ClientId == createTaskGroup.ClientId
                    && cs.ServiceId == createTaskGroup.ServiceId);

                if (clientService is null)
                {
                    clientService = new ClientService
                    {
                        ClientId = createTaskGroup.ClientId,
                        ServiceId = createTaskGroup.ServiceId
                    };
                    await context.AddAsync(clientService);
                    await context.SaveChangesAsync();
                }

                // Check if task group already exists for this month/year
                var existingGroup = await context.TaskGroups
                    .FirstOrDefaultAsync(tg => tg.ClientServiceId == clientService.Id
                    && tg.Month == DateTime.Now.Month
                    && tg.Year == DateTime.Now.Year);

                if (existingGroup != null)
                    throw new AlreadyExistObjectException("مجموعة التاسكات لهذا الشهر موجودة بالفعل");
                var taskGroup = new TaskGroup
                {
                    ClientServiceId = clientService.Id,
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
                        var task = new TaskItem
                        {
                            Title = taskDto.Title,
                            TaskType = taskDto.TaskType,
                            Description = taskDto.Description,
                            ClientServiceId = clientService.Id,
                            Deadline = taskDto.Deadline,
                            Priority = taskDto.Priority,
                            Refrence = taskDto.Refrence,
                            EmployeeId = taskDto.EmployeeId ?? string.Empty,
                            TaskGroupId = taskGroup.Id
                        };

                        context.Tasks.Add(task);
                        await context.SaveChangesAsync();

                        if (!string.IsNullOrEmpty(task.EmployeeId))
                        {
                            var employee = await userManager.FindByIdAsync(task.EmployeeId)
                                ?? throw new InvalidObjectException($"Employee with ID {task.EmployeeId} not found");
                            Dictionary<string, string> replacements = new Dictionary<string, string>
                            {
                                {"FullName", $"{employee.FirstName} {employee.LastName}" },
                                {"Email", $"{task.Employee.Email}" },
                                {"TaskTitle", $"{task.Title}" },
                                {"TaskType", $"{task.TaskType}" },
                                {"TaskId", $"{task.Id}" },
                                {"Client", $"{task.ClientService.Client.Name}" },
                                {"TimeStamp", $"{DateTime.UtcNow}" }
                            };
                            await emailService.SendEmailWithTemplateAsync(employee.Email, "New Task Has been Assigned To You", "NewTaskAssignment", replacements);

                        }

                        string? channel = task.ClientService?.Client?.DiscordChannelId;
                        if (!string.IsNullOrEmpty(channel))
                        {
                            TaskDTO mappedTask = mapper.Map<TaskDTO>(task);
                            await discordService.NewTask(channel, mappedTask);
                        }
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
        public async Task<TaskDTO> ToggleArchiveTaskAsync(int taskId)
        {
            var task = await GetTaskOrThrow(taskId);
            task.IsArchived = !task.IsArchived;
            context.Update(task);
            await context.SaveChangesAsync();
            return mapper.Map<TaskDTO>(task);
        }
        private async Task<TaskItem> GetTaskOrThrow(int taskId)
        {
            var task = await context.Tasks
                .Include(e => e.Employee)
                .FirstOrDefaultAsync(t => t.Id == taskId)
                ?? throw new InvalidObjectException("لا يوجد تاسك بهذه البيانات");

            return task;
        }

        public async Task<List<TaskDTO>> SearchTasks(string query, string currentUserId)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<TaskDTO>();

            query = query.Trim();
            var isId = int.TryParse(query, out int taskId);

            var emp = await userManager.FindByIdAsync(currentUserId)
                ?? throw new Exception("Employee not found.");

            var roles = await userManager.GetRolesAsync(emp);

            IQueryable<TaskItem> tasksQuery = context.Tasks
                .Include(t => t.ClientService)
                    .ThenInclude(cs => cs.Service)
                .Include(t => t.ClientService)
                    .ThenInclude(cs => cs.Client)
                .Include(t => t.Employee);

            // Role-based filtering
            if (roles.Contains("Admin") || roles.Contains("AccountManager"))
            {
                // no extra filter
            }
            else if (roles.Contains("ContentCreatorTeamLeader"))
            {
                var contentTypes = new[] { TaskType.ContentWriting, TaskType.ContentStrategy };
                tasksQuery = tasksQuery.Where(t => contentTypes.Contains(t.TaskType));
            }
            else if (roles.Contains("GraphicDesignerTeamLeader"))
            {
                var graphicTypes = new[]
                {
                    TaskType.Illustrations,
                    TaskType.LogoDesign,
                    TaskType.VisualIdentity,
                    TaskType.DesignDirections,
                    TaskType.SM_Design,
                    TaskType.Motion,
                    TaskType.VideoEditing
                };
                tasksQuery = tasksQuery.Where(t => graphicTypes.Contains(t.TaskType));
            }
            else
            {
                // Regular employee
                tasksQuery = tasksQuery.Where(t => t.EmployeeId == currentUserId);
            }

            // Search condition
            tasksQuery = tasksQuery.Where(t =>
                EF.Functions.Like(t.Title, $"%{query}%") ||
                (isId && t.Id == taskId));

            var tasks = await tasksQuery
                .OrderByDescending(t => t.Id)
                .ToListAsync();

            return mapper.Map<List<TaskDTO>>(tasks);
        }
        public async Task<TaskDTO> CompleteTaskAsync(int taskId, CreateTaskResourcesDTO taskResourcesDTO, string userId)
        {
            var task = await GetTaskOrThrow(taskId);
            var emp = await userManager.FindByIdAsync(userId)
                ?? throw new Exception("لم يتم العثور على هذا الموظف");

            if (taskResourcesDTO.URLs == null || !taskResourcesDTO.URLs.Any())
                throw new Exception("يجب ادخال رابط واحد على الاقل");

            var taskLinks = taskResourcesDTO.URLs
                .Select(url => new TaskCompletionResources
                {
                    TaskId = taskId,
                    URL = url
                }).ToList();

            task.CompletionNotes = taskResourcesDTO.CompletionNotes;
            task.Status = CustomTaskStatus.Completed;
            task.CompletedAt = DateTime.UtcNow;

            await context.AddRangeAsync(taskLinks);
            context.Update(task);

            if (!string.IsNullOrEmpty(task.Employee.Email))
            {
                Dictionary<string, string> replacements = new Dictionary<string, string>
                {
                    { "FullName", $"{task.Employee.FirstName} {task.Employee.LastName}" },
                    { "TaskTitle", task.Title },
                    { "TaskType", task.TaskType.ToString() },
                    { "TaskId", $"{task.Id}" },
                    { "CompletionTime", $"{task.CompletedAt}" }
                };

                await emailService.SendEmailWithTemplateAsync(task.Employee.Email, "Task Completion", "TaskCompletion", replacements);
            }
            string? channel = task.ClientService?.Client?.DiscordChannelId;
            if (!string.IsNullOrEmpty(channel))
            {
                TaskDTO mappedTask = mapper.Map<TaskDTO>(task);
                await discordService.CompleteTask(channel, mappedTask);
            }
            await context.SaveChangesAsync();
            return mapper.Map<TaskDTO>(task);
        }
    }
}
