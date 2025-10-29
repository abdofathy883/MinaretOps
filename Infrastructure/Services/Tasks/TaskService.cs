using AutoMapper;
using Core.DTOs.Notifications;
using Core.DTOs.Payloads;
using Core.DTOs.Tasks;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Infrastructure.Services.Tasks
{
    public class TaskService : ITaskService
    {
        private readonly MinaretOpsDbContext context;
        private readonly TaskHelperService helperService;
        private readonly INotificationService notificationService;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;
        public TaskService(
            MinaretOpsDbContext minaret,
            TaskHelperService taskHelper,
            INotificationService _notificationService,
            IMapper _mapper,
            UserManager<ApplicationUser> manager
            )
        {
            context = minaret;
            notificationService = _notificationService;
            mapper = _mapper;
            userManager = manager;
            helperService = taskHelper;
        }
        public async Task<bool> ChangeTaskStatusAsync(int taskId, CustomTaskStatus status, string userId)
        {
            if (status == CustomTaskStatus.Completed)
                throw new InvalidOperationException("لا يمكن انهاء التاسك من خلال هذا الاجراء");

            var user = await helperService.GetUserOrThrow(userId)
                ?? throw new Exception();
            var task = await helperService.GetTaskOrThrow(taskId);
            var emp = await helperService.GetUserOrThrow(task.EmployeeId)
                ?? throw new Exception();

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
                    UpdatedById = user.Id,
                    UpdatedAt = DateTime.UtcNow
                };
                context.Tasks.Update(task);
                await context.TaskHistory.AddAsync(history);
                if (!string.IsNullOrEmpty(emp.Email))
                {
                    var emailPayload = new
                    {
                        To = emp.Email,
                        Subject = "Task Updates",
                        Template = "ChangeTaskStatus",
                        Replacements = new Dictionary<string, string>
                        {
                            {"FullName", $"{emp.FirstName} {emp.LastName}" },
                            {"Email", $"{emp.Email}" },
                            {"TaskTitle", $"{task.Title}" },
                            {"TaskType", $"{task.TaskType}" },
                            {"TaskId", $"{task.Id}" },
                            {"OldStatus", $"{task.Status}" },
                            {"NewStatus", $"{status}" },
                            {"TimeStamp", $"{DateTime.UtcNow}" }
                        }
                    };
                    await helperService.AddOutboxAsync(OutboxTypes.Email, "Task Updates", emailPayload);
                }
                string? channel = task.ClientService?.Client?.DiscordChannelId;
                if (!string.IsNullOrEmpty(channel))
                {
                    var discordPayload = new DiscordPayload(channel, task, DiscordOperationType.ChangeTaskStatus, status);
                    await helperService.AddOutboxAsync(OutboxTypes.Discord, "Task Updates Discord", discordPayload);
                }
                var notification = new CreateNotificationDTO
                {
                    Title = $"New Status Update - {task.Title}",
                    Body = $"Task Status Updated From {oldStatus} To {status}",
                    UserId = emp.Id,
                    Url = $"https://internal.theminaretagency.com/tasks/{task.Id}"
                };
                await notificationService.CreateAsync(notification);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<bool> DeleteTaskAsync(int taskId)
        {
            var task = await helperService.GetTaskOrThrow(taskId);
            var emp = await helperService.GetUserOrThrow(task.EmployeeId);

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                context.Remove(task);

                if (emp is not null && !string.IsNullOrEmpty(emp.Email))
                {
                    var emailPayload = new
                    {
                        To = emp.Email,
                        Subject = "Task Deleted",
                        Template = "DeleteTask",
                        Replacements = new Dictionary<string, string>
                        {
                            {"FullName", $"{emp.FirstName} {emp.LastName}" },
                            {"Email", $"{emp.Email}" },
                            {"TaskTitle", $"{task.Title}" },
                            {"TaskType", $"{task.TaskType}" },
                            {"TaskId", $"{task.Id}" },
                            {"TimeStamp", $"{DateTime.UtcNow}" }
                        }
                    };
                    await helperService.AddOutboxAsync(OutboxTypes.Email, "Task Deletion Email", emailPayload);
                }
                string? channel = task.ClientService?.Client?.DiscordChannelId;
                if (!string.IsNullOrEmpty(channel))
                {
                    var discordPayload = new DiscordPayload(channel, task, DiscordOperationType.DeleteTask);
                    await helperService.AddOutboxAsync(OutboxTypes.Discord, "Task Updates Discord", discordPayload);
                }
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch(Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

        }
        public async Task<List<TaskDTO>> GetAllArchivedTasksAsync()
        {
            var tasks = await context.ArchivedTasks
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
            // First, get the main task with basic includes
            var task = await context.Tasks
                .Include(t => t.ClientService)
                    .ThenInclude(cs => cs.Service)
                .Include(t => t.ClientService)
                    .ThenInclude(cs => cs.Client)
                .Include(t => t.Employee)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null)
                throw new Exception();

            // Load related data separately if needed
            await context.Entry(task)
                .Collection(t => t.TaskHistory)
                .Query()
                .Include(th => th.UpdatedBy)
                .LoadAsync();

            await context.Entry(task)
                .Collection(t => t.CompletionResources)
                .LoadAsync();

            await context.Entry(task)
                .Collection(t => t.TaskComments)
                .LoadAsync();

            return mapper.Map<TaskDTO>(task);
        }
        public async Task<List<TaskDTO>> GetTasksByEmployeeIdAsync(string empId)
        {
            var emp = await helperService.GetUserOrThrow(empId);

            var roles = await userManager.GetRolesAsync(emp);

            IQueryable<TaskItem> query = context.Tasks
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
            var task = await helperService.GetTaskOrThrow(taskId);
            var user = await helperService.GetUserOrThrow(userId)
                ?? throw new Exception();
            var emp = await helperService.GetUserOrThrow(updateTask.EmployeeId)
                ?? throw new Exception();
            var histories = new List<TaskItemHistory>();

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                // --- Title ---
                if (!string.IsNullOrWhiteSpace(updateTask.Title) && updateTask.Title != task.Title)
                {
                    histories.Add(new TaskItemHistory
                    {
                        TaskItemId = task.Id,
                        PropertyName = "العنوان",
                        OldValue = task.Title,
                        NewValue = updateTask.Title,
                        UpdatedById = user.Id,
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
                        UpdatedById = user.Id,
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
                        UpdatedById = user.Id,
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
                        UpdatedById = user.Id,
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
                        UpdatedById = user.Id,
                        UpdatedAt = DateTime.UtcNow
                    });
                    task.Refrence = updateTask.Refrence;
                }

                // In UpdateTaskAsync before applying status change:
                if (updateTask.Status == CustomTaskStatus.Completed)
                    throw new InvalidOperationException("لا يمكن إنهاء التاسك من خلال هذا الإجراء. استخدم إكمال التاسك.");

                // --- Status ---
                if (updateTask.Status != task.Status)
                {
                    histories.Add(new TaskItemHistory
                    {
                        TaskItemId = task.Id,
                        PropertyName = "الحالة",
                        OldValue = task.Status.ToString(),
                        NewValue = updateTask.Status.ToString(),
                        UpdatedById = user.Id,
                        UpdatedAt = DateTime.UtcNow
                    });
                    task.Status = updateTask.Status;
                }

                // --- Employee ---
                if (!string.IsNullOrWhiteSpace(updateTask.EmployeeId) && updateTask.EmployeeId != task.EmployeeId)
                {
                    histories.Add(new TaskItemHistory
                    {
                        TaskItemId = task.Id,
                        PropertyName = "الموظف",
                        OldValue = task.Employee != null ? $"{task.Employee.FirstName} {task.Employee.LastName}" : string.Empty,
                        NewValue = $"{emp.FirstName} {emp.LastName}",
                        UpdatedById = userId,
                        UpdatedAt = DateTime.UtcNow
                    });

                    task.EmployeeId = emp.Id;
                }

                // --- Save task + history in one transaction ---
                context.Tasks.Update(task);
                if (histories.Any())
                    await context.TaskHistory.AddRangeAsync(histories);

                // --- Notifications ---
                if (!string.IsNullOrEmpty(emp.Email))
                {
                    var emailPayload = new
                    {
                        To = emp.Email,
                        Subject = "Task Has Been Updated",
                        Template = "TaskUpdates",
                        Replacements = new Dictionary<string, string>
                        {
                            {"FullName", $"{emp.FirstName} {emp.LastName}" },
                            {"Email", emp.Email },
                            {"TaskTitle", task.Title },
                            {"TaskType", task.TaskType.ToString() },
                            {"TaskId", task.Id.ToString() },
                            {"TimeStamp", DateTime.UtcNow.ToString("u") }
                        }
                    };
                    await helperService.AddOutboxAsync(OutboxTypes.Email, "Task Has Been Updated", emailPayload);
                }

                // --- Discord update ---
                string? channel = task.ClientService?.Client?.DiscordChannelId;
                if (!string.IsNullOrEmpty(channel))
                {
                    var discordPayload = new DiscordPayload(channel, task, DiscordOperationType.UpdateTask);
                    await helperService.AddOutboxAsync(OutboxTypes.Discord, "Task Updates Discord", discordPayload);
                }
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return mapper.Map<TaskDTO>(task);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<TaskDTO> CreateTaskAsync(string userId, CreateTaskDTO createTask)
        {
            var user = await userManager.FindByIdAsync(userId)
                ?? throw new InvalidObjectException("لم يتم العثور على المستخدم الحالي");
            // Validate that the task group exists
            var taskGroup = await context.TaskGroups
                .FirstOrDefaultAsync(tg => tg.Id == createTask.TaskGroupId)
                ?? throw new InvalidObjectException("لم نتمكن من العثور على مجموعة التاسكات");

            // Validate that the client service exists
            var clientService = await context.ClientServices
                .Include(cs => cs.Client)
                .FirstOrDefaultAsync(cs => cs.Id == createTask.ClientServiceId)
                ?? throw new InvalidObjectException("العميل غير مشترك في هذه الخدمة");

            ApplicationUser? emp = null;
            var normalizedEmployeeId = string.IsNullOrWhiteSpace(createTask.EmployeeId)
                ? null : createTask.EmployeeId;

            if (normalizedEmployeeId is not null)
            {
                emp = await helperService.GetUserOrThrow(normalizedEmployeeId);
            }

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
                    EmployeeId = normalizedEmployeeId,
                    TaskGroupId = createTask.TaskGroupId
                };

                await context.Tasks.AddAsync(task);

                var taskHistory = new TaskItemHistory
                {
                    TaskItem = task,
                    PropertyName = "انشاء التاسك",
                    UpdatedById = user.Id,
                    OldValue = "لا يوجد",
                    NewValue = "لا يوجد",
                    UpdatedBy = user,
                    UpdatedAt = DateTime.UtcNow
                };

                await context.AddAsync(taskHistory);

                if (emp is not null && !string.IsNullOrEmpty(emp.Email))
                {
                    var emailPayload = new
                    {
                        To = emp.Email,
                        Subject = "New Task Has Been Assigned To You",
                        Template = "NewTaskAssignment",
                        Replacements = new Dictionary<string, string>
                        {
                            {"TaskTitle", $"{task.Title}" },
                            {"TaskType", $"{task.TaskType}" },
                            {"TaskId", $"{task.Id}" },
                            {"TimeStamp", $"{DateTime.UtcNow}" }
                        }
                    };
                    await helperService.AddOutboxAsync(OutboxTypes.Email, "Send New Task Email", emailPayload);
                }

                string? channel = task.ClientService?.Client?.DiscordChannelId;
                if (channel is not null)
                {
                    var discordPayload = new DiscordPayload(channel, task, DiscordOperationType.NewTask);
                    await helperService.AddOutboxAsync(OutboxTypes.Discord, "Send New Discord Notification", discordPayload);
                }
                await context.SaveChangesAsync();

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
            catch(Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<TaskGroupDTO> CreateTaskGroupAsync(CreateTaskGroupDTO createTaskGroup, string userId)
        {
            var user = await helperService.GetUserOrThrow(userId)
                ?? throw new Exception();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                // Validate that the client service exists
                var clientService = await context.ClientServices
                    .FirstOrDefaultAsync(cs => cs.ClientId == createTaskGroup.ClientId
                    && cs.ServiceId == createTaskGroup.ServiceId);

                if (clientService is null)
                {
                    var client = await context.Clients.FindAsync(createTaskGroup.ClientId)
                        ?? throw new Exception();
                    clientService = new ClientService
                    {
                        ClientId = createTaskGroup.ClientId,
                        Client = client,
                        ServiceId = createTaskGroup.ServiceId
                    };
                    await context.AddAsync(clientService);
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
                    ClientService = clientService,
                    Month = DateTime.Now.Month,
                    Year = DateTime.Now.Year,
                    MonthLabel = $"{DateTime.Now.ToString("MMMM")} {DateTime.Now.ToString("yyyy")}"
                };

                await context.TaskGroups.AddAsync(taskGroup);

                // Create tasks if provided
                if (createTaskGroup.Tasks.Any())
                {
                    foreach (var taskDto in createTaskGroup.Tasks)
                    {
                        ApplicationUser? employee = null;
                        var normalizedEmployeeId = string.IsNullOrWhiteSpace(taskDto.EmployeeId)
                            ? null : taskDto.EmployeeId;

                        if (normalizedEmployeeId is not null)
                        {
                            employee = await helperService.GetUserOrThrow(normalizedEmployeeId);
                        }
                        var task = new TaskItem
                        {
                            Title = taskDto.Title,
                            TaskType = taskDto.TaskType,
                            Description = taskDto.Description,
                            ClientServiceId = clientService.Id,
                            ClientService = clientService,
                            Deadline = taskDto.Deadline,
                            Priority = taskDto.Priority,
                            Refrence = taskDto.Refrence,
                            EmployeeId = normalizedEmployeeId,
                            TaskGroup = taskGroup
                        };

                        context.Tasks.Add(task);

                        var taskHistory = new TaskItemHistory
                        {
                            TaskItem = task,
                            PropertyName = "انشاء التاسك",
                            UpdatedById = user.Id,
                            UpdatedBy = user,
                            UpdatedAt = DateTime.UtcNow
                        };

                        if (employee is not null && !string.IsNullOrEmpty(employee.Email))
                        {
                            var emailPayload = new
                            {
                                To = employee.Email,
                                Subject = "New Task Has Been Assigned To You",
                                Template = "NewTaskAssignment",
                                Replacements = new Dictionary<string, string>
                                {
                                    {"FullName", $"{employee.FirstName} {employee.LastName}" },
                                    {"Email", $"{employee.Email}" },
                                    {"TaskTitle", $"{task.Title}" },
                                    {"TaskType", $"{task.TaskType}" },
                                    {"TaskId", $"{task.Id}" },
                                    {"Client", $"{task.ClientService.Client.Name}" },
                                    {"TimeStamp", $"{DateTime.UtcNow}" }
                                }
                            };
                            await helperService.AddOutboxAsync(OutboxTypes.Email, "Send New Task Email", emailPayload);
                        }

                        string? channel = task.ClientService?.Client?.DiscordChannelId;
                        if (!string.IsNullOrEmpty(channel))
                        {
                            var discordPayload = new DiscordPayload(channel, task, DiscordOperationType.NewTask);
                            await helperService.AddOutboxAsync(OutboxTypes.Discord, "Send New Discord Notification", discordPayload);
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

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return mapper.Map<TaskGroupDTO>(createdTaskGroup);
            }
            catch(Exception)
            {
                await transaction.RollbackAsync();
                throw;
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

        public async Task<List<TaskDTO>> SearchTasks(string query, string currentUserId)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<TaskDTO>();

            query = query.Trim();
            var isId = int.TryParse(query, out int taskId);

            var emp = await helperService.GetUserOrThrow(currentUserId);

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
            var task = await helperService.GetTaskOrThrow(taskId);
            var user = await userManager.FindByIdAsync(userId)
                ?? throw new Exception("لم يتم العثور على هذا الموظف");

            var emp = await helperService.GetUserOrThrow(task.EmployeeId);

            if (taskResourcesDTO.URLs == null || !taskResourcesDTO.URLs.Any())
                throw new Exception("يجب ادخال رابط واحد على الاقل");

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
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

                if (emp is not null && !string.IsNullOrEmpty(emp.Email))
                {
                    var emailPayload = new
                    {
                        To = emp.Email,
                        Subject = "Task Completion",
                        Template = "TaskCompletion",
                        Replacements = new Dictionary<string, string>
                        {
                            { "FullName", $"{emp.FirstName} {emp.LastName}" },
                            { "TaskTitle", task.Title },
                            { "TaskType", task.TaskType.ToString() },
                            { "TaskId", $"{task.Id}" },
                            { "CompletionTime", $"{task.CompletedAt}" }
                        }
                    };
                    await helperService.AddOutboxAsync(OutboxTypes.Email, "Task Completion Email", emailPayload);
                }

                string? channel = task.ClientService?.Client?.DiscordChannelId;
                if (!string.IsNullOrEmpty(channel))
                {
                    var discordPayload = new DiscordPayload(channel, task, DiscordOperationType.CompleteTask);
                    await helperService.AddOutboxAsync(OutboxTypes.Discord, "Task Updates Discord", discordPayload);
                }
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
        public async Task<PaginatedTaskResultDTO> GetPaginatedTasksAsync(TaskFilterDTO filter, string currentUserId)
        {
            var emp = await helperService.GetUserOrThrow(currentUserId);
            var roles = await userManager.GetRolesAsync(emp);

            IQueryable<TaskItem> query = context.Tasks
                .Include(t => t.ClientService)
                    .ThenInclude(cs => cs.Service)
                .Include(t => t.ClientService)
                    .ThenInclude(cs => cs.Client)
                .Include(t => t.Employee);

            // Role-based filtering
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
                query = query.Where(t => t.EmployeeId == currentUserId);
            }

            // Apply team filter
            if (!string.IsNullOrEmpty(filter.Team) && filter.Team != "all")
            {
                var teamTaskTypes = GetTaskTypesForTeam(filter.Team);
                if (teamTaskTypes.Any())
                {
                    query = query.Where(t => teamTaskTypes.Contains(t.TaskType));
                }
            }

            // Apply date range filter (using CreatedAt for task creation date)
            if (filter.FromDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt.Date >= filter.FromDate.Value.Date);
            }

            if (filter.ToDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt.Date <= filter.ToDate.Value.Date);
            }

            // Apply employee filter
            if (!string.IsNullOrEmpty(filter.EmployeeId))
            {
                query = query.Where(t => t.EmployeeId == filter.EmployeeId);
            }

            // Apply client filter
            if (filter.ClientId.HasValue)
            {
                query = query.Where(t => t.ClientService.ClientId == filter.ClientId.Value);
            }

            // Apply status filter
            if (filter.Status.HasValue)
            {
                query = query.Where(t => (int)t.Status == filter.Status.Value);
            }

            // Apply priority filter
            if (!string.IsNullOrEmpty(filter.Priority))
            {
                query = query.Where(t => t.Priority == filter.Priority);
            }

            // Apply deadline filter
            if (!string.IsNullOrEmpty(filter.OnDeadline))
            {
                var currentDate = DateTime.UtcNow;

                if (filter.OnDeadline == "yes")
                {
                    // Task completed on deadline: must have completedAt AND isCompletedOnDeadline = true
                    query = query.Where(t => t.CompletedAt.HasValue &&
                        t.CompletedAt.Value <= t.Deadline);
                }
                else if (filter.OnDeadline == "no")
                {
                    // Task completed late OR deadline passed but not completed
                    query = query.Where(t =>
                        (t.CompletedAt.HasValue && t.CompletedAt.Value > t.Deadline) ||
                        (!t.CompletedAt.HasValue && currentDate >= t.Deadline));
                }
                else if (filter.OnDeadline == "not-yet")
                {
                    // Task not completed and deadline hasn't arrived yet
                    query = query.Where(t => !t.CompletedAt.HasValue && currentDate < t.Deadline);
                }
            }

            // Get total count before pagination
            var totalRecords = await query.CountAsync();

            // Apply ordering and pagination
            var tasks = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalRecords / (double)filter.PageSize);

            return new PaginatedTaskResultDTO
            {
                Records = mapper.Map<List<TaskDTO>>(tasks),
                TotalRecords = totalRecords,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalPages = totalPages
            };
        }
        private TaskType[] GetTaskTypesForTeam(string team)
        {
            return team switch
            {
                "content-creation" => new[]
                {
                    TaskType.ContentStrategy,
                    TaskType.ContentWriting
                },
                "video" => new[]
                {
                    TaskType.Voiceover,
                    TaskType.VideoEditing,
                    TaskType.Motion
                },
                "seo" => new[]
                {
                    TaskType.SEO
                },
                "ads" => new[]
                {
                    TaskType.Ad_Management,
                    TaskType.E_mailMarketing,
                    TaskType.WhatsAppMarketing
                },
                "design" => new[]
                {
                    TaskType.LogoDesign,
                    TaskType.VisualIdentity,
                    TaskType.DesignDirections,
                    TaskType.SM_Design,
                    TaskType.PrintingsDesign,
                    TaskType.Illustrations,
                    TaskType.UI_UX
                },
                "software" => new[]
                {
                    TaskType.WordPress,
                    TaskType.Backend,
                    TaskType.Frontend
                },
                "management" => new[]
                {
                    TaskType.Planning,
                    TaskType.Meeting,
                    TaskType.HostingManagement,
                    TaskType.Publishing,
                    TaskType.Moderation
                },
                _ => Array.Empty<TaskType>()
            };
        }
        public async Task<TaskCommentDTO> AddCommentAsync(CreateTaskCommentDTO taskComment)
        {
            var task = await helperService.GetTaskOrThrow(taskComment.TaskId)
                ?? throw new Exception();
            var employee = await helperService.GetUserOrThrow(taskComment.EmployeeId)
                ?? throw new Exception();
            if (string.IsNullOrEmpty(taskComment.Comment))
                throw new Exception();

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var newComment = new TaskComment
                {
                    Comment = taskComment.Comment,
                    TaskId = task.Id,
                    EmployeeId = employee.Id,
                    CreatedAt = DateTime.UtcNow
                };
                await context.TaskComments.AddAsync(newComment);

                var emailPayload = new
                {
                    To = employee.Email,
                    Subject = "New Comment On Task Assigned To You",
                    Template = "NewTaskComment",
                    Replacements = new Dictionary<string, string>
                    {
                        { "TaskId", task.Id.ToString() },
                        { "TaskTitle", task.Title },
                        { "TaskComment", taskComment.Comment },
                        { "ByEmployee", $"{employee.FirstName} {employee.LastName}" },
                        { "TimeSpan", DateTime.Now.ToString() }
                    }
                };
                await helperService.AddOutboxAsync(OutboxTypes.Email, "New Task Comment Email", emailPayload);

                string? channel = task?.ClientService?.Client?.DiscordChannelId;
                if (channel is not null)
                {
                    var discordPayload = new DiscordPayload(channel, task, DiscordOperationType.NewComment);
                    await helperService.AddOutboxAsync(OutboxTypes.Discord, "New Task Comment Discord", discordPayload);
                }
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return mapper.Map<TaskCommentDTO>(newComment);
            }
            catch(Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
