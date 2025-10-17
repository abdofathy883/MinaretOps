using AutoMapper;
using Core.DTOs.InternalTasks;
using Core.DTOs.Tasks;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Infrastructure.Services.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Infrastructure.Services.InternalTasks
{
    public class InternalTaskService : IInternalTaskService
    {
        private readonly MinaretOpsDbContext context;
        private readonly TaskHelperService helperService;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;
        public InternalTaskService(
            MinaretOpsDbContext minaret,
            TaskHelperService _helperService,
            IMapper _mapper,
            UserManager<ApplicationUser> manager
            )
        {
            context = minaret;
            helperService = _helperService;
            mapper = _mapper;
            userManager = manager;
        }
        public async Task<bool> ChangeTaskStatusAsync(int taskId, CustomTaskStatus status)
        {
            var task = await GetTaskOrThrow(taskId);
            //var emp = await helperService.GetUserOrThrow(task.Assignments.)

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                task.Status = status;
                if (status == CustomTaskStatus.Completed)
                {
                    task.CompletedAt = DateTime.UtcNow;
                }
                context.Update(task);
                foreach (var ass in task.Assignments)
                {
                    if (!string.IsNullOrEmpty(ass.User.Email))
                    {
                        var emailPayload = new
                        {
                            To = ass.User.Email,
                            Subject = "Internal Task Status Changes",
                            Template = "ChangeTaskStatus",
                            Replacements = new Dictionary<string, string>
                            {
                                { "TaskTitle", task.Title },
                                { "TaskId", $"{task.Id}" },
                                { "TaskType", $"{task.TaskType}" },
                                { "TaskStatus", status.ToString() },
                                { "TaskDeadline", task.Deadline.ToString("yyyy-MM-dd") }
                            }
                        };
                        await helperService.AddOutboxAsync(OutboxTypes.Email, "Internal Task Email", emailPayload);
                    }
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
        public async Task<InternalTaskDTO> CreateInternalTaskAsync(CreateInternalTaskDTO internalTaskDTO)
        {
            var userIds = internalTaskDTO.Assignments.Select(a => a.UserId).Distinct().ToList();
            var existingUserIds = await context.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => u.Id)
                .ToListAsync();

            var missingUsers = userIds.Except(existingUserIds).ToList();
            if (missingUsers.Count > 0)
                throw new ArgumentException($"Some users were not found: {string.Join(", ", missingUsers)}");

            // Leader validation: if more than one assignment, exactly one leader must be set
            var leadersCount = internalTaskDTO.Assignments.Count(a => a.IsLeader);
            if (internalTaskDTO.Assignments.Count > 1)
            {
                if (leadersCount != 1)
                    throw new ArgumentException("Exactly one leader must be assigned when more than one user is assigned.");
            }
            else if (internalTaskDTO.Assignments.Count == 1 && leadersCount == 0)
            {
                // Auto-promote the single assignee to leader
                internalTaskDTO.Assignments[0].IsLeader = true;
            }

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var task = new InternalTask
                {
                    Title = internalTaskDTO.Title,
                    TaskType = internalTaskDTO.TaskType,
                    Description = internalTaskDTO.Description,
                    Deadline = internalTaskDTO.Deadline,
                    Priority = string.IsNullOrWhiteSpace(internalTaskDTO.Priority) ? "عادي" : internalTaskDTO.Priority,
                    Status = CustomTaskStatus.Open,
                    Assignments = new List<InternalTaskAssignment>()
                };

                await context.AddAsync(task);

                foreach (var assignment in internalTaskDTO.Assignments)
                {
                    var taskAssignment = new InternalTaskAssignment
                    {
                        Task = task,
                        UserId = assignment.UserId,
                        IsLeader = assignment.IsLeader
                    };
                    await context.AddAsync(taskAssignment);
                }
                
                var assignmentsWithUsers = await context.InternalTaskAssignments
                    .Where(a => a.InternalTaskId == task.Id)
                    .Include(a => a.User)
                    .ToListAsync();

                foreach (var ass in assignmentsWithUsers)
                {
                    if (!string.IsNullOrEmpty(ass.User.Email))
                    {
                        var emailPayload = new
                        {
                            To = ass.User.Email,
                            Subject = "New Internal Task",
                            Template = "NewTaskAssignment",
                            Replacements = new Dictionary<string, string>
                            {
                                { "TaskTitle", task.Title },
                                { "TaskId", $"{task.Id}" },
                                { "TaskType", $"{task.TaskType}" },
                                { "TaskStatus", task.Status.ToString() },
                                { "TaskDeadline", task.Deadline.ToString("yyyy-MM-dd") }
                            }
                        };
                        await helperService.AddOutboxAsync(OutboxTypes.Email, "New Internal Task Email", emailPayload);
                    }
                }
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return mapper.Map<InternalTaskDTO>(task);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

        }
        public async Task<bool> DeleteInternalTaskAsync(int taskId)
        {
            var task = await GetTaskOrThrow(taskId);

            foreach (var ass in task.Assignments)
            {
                context.Remove(ass);
                await context.SaveChangesAsync();
            }
            context.Remove(task);
            return await context.SaveChangesAsync() > 0;
        }
        public async Task<List<InternalTaskDTO>> GetAllArchivedInternalTasksAsync()
        {
            var tasks = await context.InternalTasks
                .Where(t => t.IsArchived)
                .Include(t => t.Assignments)
                    .ThenInclude(a => a.User)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return mapper.Map<List<InternalTaskDTO>>(tasks);
        }
        public async Task<InternalTaskDTO> GetInternalTaskById(int taskId)
        {
            var task = await GetTaskOrThrow(taskId);

            return mapper.Map<InternalTaskDTO>(task);
        }
        public async Task<List<InternalTaskDTO>> GetInternalTasksByEmpAsync(string empId)
        {
            var emp = await userManager.FindByIdAsync(empId)
                ?? throw new Exception("لم يتم العثور على موظف بهذا المعرف");

            var tasks = await context.InternalTasks
                .Where(it => !it.IsArchived)
                .Include(it => it.Assignments)
                    .ThenInclude(a => a.User)
                .Where(it => it.Assignments.Any(a => a.UserId == empId))
                .ToListAsync();

            return mapper.Map<List<InternalTaskDTO>>(tasks);
        }
        public async Task<List<InternalTaskDTO>> SearchByTitleAsync(string query, string empId)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<InternalTaskDTO>();

            query = query.Trim();
            var isId = int.TryParse(query, out int taskId);

            IQueryable<InternalTask> tasksQuery = context.InternalTasks
                .Include(t => t.Assignments)
                .Where(it => it.Assignments.Any(a => a.UserId == empId));

            tasksQuery = tasksQuery.Where(t =>
                EF.Functions.Like(t.Title, $"%{query}%") ||
                (isId && t.Id == taskId));

            var tasks = await tasksQuery
                .OrderByDescending(t => t.Id)
                .ToListAsync();

            return mapper.Map<List<InternalTaskDTO>>(tasks);
        }
        public async Task<InternalTaskDTO> UpdateInternalTaskAsync(int taskId, UpdateInternalTaskDTO internalTaskDTO)
        {
            var task = await GetTaskOrThrow(taskId);

            // Validate users existence if assignments are provided
            if (internalTaskDTO.Assignments?.Any() == true)
            {
                var userIds = internalTaskDTO.Assignments.Select(a => a.UserId).Distinct().ToList();
                var existingUserIds = await context.Users
                    .Where(u => userIds.Contains(u.Id))
                    .Select(u => u.Id)
                    .ToListAsync();

                var missingUsers = userIds.Except(existingUserIds).ToList();
                if (missingUsers.Count > 0)
                    throw new ArgumentException($"Some users were not found: {string.Join(", ", missingUsers)}");

                // Leader validation: if more than one assignment, exactly one leader must be set
                var leadersCount = internalTaskDTO.Assignments.Count(a => a.IsLeader);
                if (internalTaskDTO.Assignments.Count > 1)
                {
                    if (leadersCount != 1)
                        throw new ArgumentException("Exactly one leader must be assigned when more than one user is assigned.");
                }
                else if (internalTaskDTO.Assignments.Count == 1 && leadersCount == 0)
                {
                    // Auto-promote the single assignee to leader
                    internalTaskDTO.Assignments[0].IsLeader = true;
                }
            }

            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                // Update task properties
                task.Title = internalTaskDTO.Title;
                task.Status = internalTaskDTO.Status;
                task.Priority = internalTaskDTO.Priority;
                task.Description = internalTaskDTO.Description;
                task.Deadline = internalTaskDTO.Deadline;
                task.TaskType = internalTaskDTO.TaskType;

                // Set CompletedAt if status is Completed and it's not already set
                if (internalTaskDTO.Status == CustomTaskStatus.Completed && !task.CompletedAt.HasValue)
                {
                    task.CompletedAt = DateTime.UtcNow;
                }
                else if (internalTaskDTO.Status != CustomTaskStatus.Completed)
                {
                    task.CompletedAt = null;
                }

                context.Update(task);

                // Update assignments if provided
                if (internalTaskDTO.Assignments?.Any() == true)
                {
                    // Remove existing assignments
                    var existingAssignments = await context.InternalTaskAssignments
                        .Where(a => a.InternalTaskId == taskId)
                        .ToListAsync();

                    foreach (var assignment in existingAssignments)
                    {
                        context.Remove(assignment);
                    }

                    // Add new assignments
                    foreach (var assignmentDTO in internalTaskDTO.Assignments)
                    {
                        var taskAssignment = new InternalTaskAssignment
                        {
                            InternalTaskId = taskId,
                            UserId = assignmentDTO.UserId,
                            IsLeader = assignmentDTO.IsLeader
                        };

                        await context.AddAsync(taskAssignment);
                    }
                }

                var updatedAssignments = await context.InternalTaskAssignments
                    .Where(a => a.InternalTaskId == taskId)
                    .Include(a => a.User)
                    .ToListAsync();

                foreach (var ass in updatedAssignments)
                {
                    if (!string.IsNullOrEmpty(ass.User?.Email))
                    {
                        var emailPayload = new
                        {
                            To = ass.User.Email,
                            Subject = "Updated Internal Task",
                            Template = "TaskUpdates",
                            Replacements = new Dictionary<string, string>
                            {
                                { "TaskTitle", task.Title },
                                { "TaskId", $"{task.Id}" },
                                { "TaskType", $"{task.TaskType}" },
                                { "TaskStatus", task.Status.ToString() },
                                { "TaskDeadline", task.Deadline.ToString("yyyy-MM-dd") }
                            }
                        };
                        await helperService.AddOutboxAsync(OutboxTypes.Email, "Internal Task Update Email", emailPayload);
                    }
                }
                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Return updated task with assignments
                var updatedTask = await context.InternalTasks
                    .Include(t => t.Assignments)
                        .ThenInclude(a => a.User)
                    .FirstOrDefaultAsync(t => t.Id == taskId);

                return mapper.Map<InternalTaskDTO>(updatedTask);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<InternalTaskDTO> ToggleArchiveInternalTaskAsync(int taskId)
        {
            var task = await GetTaskOrThrow(taskId);

            task.IsArchived = !task.IsArchived;
            context.Update(task);
            await context.SaveChangesAsync();
            return mapper.Map<InternalTaskDTO>(task);
        }
        private async Task<InternalTask> GetTaskOrThrow(int taskId)
        {
            var task = await context.InternalTasks
                .Include(t => t.Assignments)
                    .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(t => t.Id == taskId)
                ?? throw new InvalidObjectException("لم يتم العثور على التاسك");
            return task;
        }
    }
}
