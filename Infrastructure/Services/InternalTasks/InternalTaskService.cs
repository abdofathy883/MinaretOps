using AutoMapper;
using Core.DTOs.InternalTasks;
using Core.DTOs.Tasks;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Infrastructure.Services.InternalTasks
{
    public class InternalTaskService : IInternalTaskService
    {
        private readonly MinaretOpsDbContext context;
        private readonly IEmailService emailService;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;
        public InternalTaskService(
            MinaretOpsDbContext minaret,
            IEmailService email,
            IMapper _mapper,
            UserManager<ApplicationUser> manager
            )
        {
            context = minaret;
            emailService = email;
            mapper = _mapper;
            userManager = manager;
        }

        public async Task<bool> ChangeTaskStatusAsync(int taskId, CustomTaskStatus status)
        {
            var task = await context.InternalTasks.FirstOrDefaultAsync(t => t.Id == taskId)
                ?? throw new InvalidObjectException("بيانات التاسك غير صحيحة");

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                task.Status = status;
                if (status == CustomTaskStatus.Completed)
                {
                    task.CompletedAt = DateTime.UtcNow;
                }

                foreach (var ass in task.Assignments)
                {
                    if (!string.IsNullOrEmpty(ass.User.Email))
                    {
                        Dictionary<string, string> replacements = new()
                        {
                            { "TaskTitle", task.Title },
                            { "TaskId", $"{task.Id}" },
                            { "TaskType", $"{task.TaskType}" },
                            { "TaskStatus", status.ToString() },
                            { "TaskDeadline", task.Deadline.ToString("yyyy-MM-dd") }
                        };
                        await emailService.SendEmailWithTemplateAsync(ass.User.Email, "Task Status Changes", "ChangeTaskStatus", replacements);
                    }
                }

                context.Update(task);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;

            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }

        }

        public async Task<InternalTaskDTO> CreateInternalTaskAsync(CreateInternalTaskDTO internalTaskDTO)
        {
            if (internalTaskDTO is null)
                throw new InvalidObjectException(nameof(internalTaskDTO));

            // Validate users existence
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
                await context.SaveChangesAsync();


                foreach (var assignment in internalTaskDTO.Assignments)
                {
                    var taskAssignment = new InternalTaskAssignment
                    {
                        InternalTaskId = task.Id,
                        UserId = assignment.UserId,
                        IsLeader = assignment.IsLeader
                    };

                    task.Assignments.Add(taskAssignment);
                    await context.AddAsync(taskAssignment);

                }
                
                await context.SaveChangesAsync();

                var assignmentsWithUsers = await context.InternalTaskAssignments
                    .Where(a => a.InternalTaskId == task.Id)
                    .Include(a => a.User)
                    .ToListAsync();

                foreach (var ass in assignmentsWithUsers)
                {
                    if (!string.IsNullOrEmpty(ass.User?.Email))
                    {
                        Dictionary<string, string> replacements = new()
                        {
                            { "TaskTitle", task.Title },
                            { "TaskId", $"{task.Id}" },
                            { "TaskType", $"{task.TaskType}" },
                            { "TaskStatus", task.Status.ToString() },
                            { "TaskDeadline", task.Deadline.ToString("yyyy-MM-dd") }
                        };
                        await emailService.SendEmailWithTemplateAsync(
                            ass.User.Email,
                            "New Internal Task",
                            "NewTaskAssignment",
                            replacements
                        );
                    }
                }
                await transaction.CommitAsync();
                return mapper.Map<InternalTaskDTO>(task);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new NotImplementedOperationException($"حدث خطا اثناء اضافة التاسك, {ex.Message}");
            }

        }

        public async Task<bool> DeleteInternalTaskAsync(int taskId)
        {
            var task = await context.InternalTasks
                .SingleOrDefaultAsync(t => t.Id == taskId)
                ?? throw new Exception();

            var assignments = await context.InternalTaskAssignments
                .Where(t => t.InternalTaskId == taskId)
                .ToListAsync();

            foreach (var ass in assignments)
            {
                context.Remove(ass);
                await context.SaveChangesAsync();
            }
            context.Remove(task);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<List<InternalTaskDTO>> GetAllUnArchivedInternalTasksAsync()
        {
            var tasks = await context.InternalTasks
                .Where(t => !t.IsArchived)
                .Include(t => t.Assignments)
                    .ThenInclude(a => a.User)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return mapper.Map<List<InternalTaskDTO>>(tasks);
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
            var task = await context.InternalTasks
                .Include(t => t.Assignments)
                    .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(t => t.Id == taskId)
                ?? throw new InvalidObjectException("لم يتم العثور على التاسك");

            return mapper.Map<InternalTaskDTO>(task);
        }

        public async Task<List<InternalTaskDTO>> GetInternalTasksByEmpAsync(string empId)
        {
            var emp = await userManager.FindByIdAsync(empId)
                ?? throw new Exception("Employee not found.");

            //IQueryable<InternalTask> query = context.InternalTasks
            //    .Where(t => !t.IsArchived)
            //    .Include(t => t.Assignments)
            //        .ThenInclude(a => a.User);

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
            if (internalTaskDTO is null)
                throw new InvalidObjectException(nameof(internalTaskDTO));

            var task = await context.InternalTasks
                .Include(t => t.Assignments)
                .SingleOrDefaultAsync(t => t.Id == taskId) ??
                    throw new InvalidObjectException("لم يتم العثور على التاسك");

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
                await context.SaveChangesAsync();

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

                await context.SaveChangesAsync();

                var updatedAssignments = await context.InternalTaskAssignments
                    .Where(a => a.InternalTaskId == taskId)
                    .Include(a => a.User)
                    .ToListAsync();

                foreach (var ass in updatedAssignments)
                {
                    if (!string.IsNullOrEmpty(ass.User?.Email))
                    {
                        Dictionary<string, string> replacements = new()
                        {
                            { "TaskTitle", task.Title },
                            { "TaskId", $"{task.Id}" },
                            { "TaskType", $"{task.TaskType}" },
                            { "TaskStatus", task.Status.ToString() },
                            { "TaskDeadline", task.Deadline.ToString("yyyy-MM-dd") }
                        };
                        await emailService.SendEmailWithTemplateAsync(
                            ass.User.Email,
                            "Updated Internal Task",
                            "TaskUpdates",
                            replacements
                        );
                    }
                }
                await transaction.CommitAsync();

                // Return updated task with assignments
                var updatedTask = await context.InternalTasks
                    .Include(t => t.Assignments)
                        .ThenInclude(a => a.User)
                    .FirstOrDefaultAsync(t => t.Id == taskId);

                return mapper.Map<InternalTaskDTO>(updatedTask);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new NotImplementedOperationException($"حدث خطأ أثناء تحديث التاسك: {ex.Message}");
            }
        }

        public async Task<bool> ToggleArchiveInternalTaskAsync(int taskId)
        {
            var task = await GetTaskOrThrow(taskId);

            task.IsArchived = !task.IsArchived;
            context.Update(task);
            return await context.SaveChangesAsync() > 0;
        }

        private async Task<InternalTask> GetTaskOrThrow(int taskId)
        {
            var task = await context.InternalTasks.FirstOrDefaultAsync(t => t.Id == taskId)
                ?? throw new Exception();
            return task;
        }
    }
}
