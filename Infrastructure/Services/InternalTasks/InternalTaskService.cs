using AutoMapper;
using Core.DTOs.InternalTasks;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.InternalTasks
{
    public class InternalTaskService : IInternalTaskService
    {
        private readonly MinaretOpsDbContext context;
        private readonly IEmailService emailService;
        private readonly IMapper mapper;
        public InternalTaskService(
            MinaretOpsDbContext minaret,
            IEmailService email,
            IMapper _mapper
            )
        {
            context = minaret;
            emailService = email;
            mapper = _mapper;
        }

        public async Task<bool> ChangeTaskStatusAsync(int taskId, CustomTaskStatus status)
        {
            var task = await context.InternalTasks.FirstOrDefaultAsync(t => t.Id == taskId)
                ?? throw new InvalidObjectException("بيانات التاسك غير صحيحة");

            task.Status = status;
            if (status == CustomTaskStatus.Completed)
            {
                task.CompletedAt = DateTime.UtcNow;
            }

            await context.SaveChangesAsync();
            return true;
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
                await transaction.CommitAsync();
                return mapper.Map<InternalTaskDTO>(task);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new NotImplementedOperationException($"حدث خطا اثناء اضافة التاسك, {ex.Message}");
            }

        }

        public async Task<List<InternalTaskDTO>> GetAllInternalTasksAsync()
        {
            var tasks = await context.InternalTasks
                .Include(t => t.Assignments)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return mapper.Map<List<InternalTaskDTO>>(tasks);
        }

        public async Task<InternalTaskDTO> GetInternalTaskById(int taskId)
        {
            var task = await context.InternalTasks
                .Include(t => t.Assignments)
                .FirstOrDefaultAsync(t => t.Id == taskId)
                ?? throw new InvalidObjectException("لم يتم العثور على التاسك");

            return mapper.Map<InternalTaskDTO>(task);
        }
    }
}
