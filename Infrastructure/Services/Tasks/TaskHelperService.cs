using Core.Enums;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Infrastructure.Services.Tasks
{
    public class TaskHelperService
    {
        private readonly MinaretOpsDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public TaskHelperService(MinaretOpsDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task AddOutboxAsync(OutboxTypes type, string title, object payload)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = false
            };

            var outbox = new Outbox
            {
                OpType = type,
                OpTitle = title,
                PayLoad = JsonSerializer.Serialize(payload, jsonOptions)
            };
            await context.OutboxMessages.AddAsync(outbox);
        }

        public async Task<TaskItem> GetTaskOrThrow(int taskId)
        {
            var task = await context.Tasks
                .Include(t => t.Employee)
                .Include(t => t.ClientService)
                    .ThenInclude(cs => cs.Client)
                .Include(t => t.ClientService)
                    .ThenInclude(cs => cs.Service)
                .FirstOrDefaultAsync(t => t.Id == taskId)
                ?? throw new InvalidObjectException("لا يوجد تاسك بهذه البيانات");

            return task;
        }
        public async Task<ApplicationUser?> GetUserOrThrow(string empId)
        {
            var user = await userManager.FindByIdAsync(empId);
            return user;
        }
    }
}
