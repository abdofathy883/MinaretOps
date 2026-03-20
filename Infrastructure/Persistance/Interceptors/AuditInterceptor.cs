using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Security.Claims;
using System.Text.Json;

namespace Infrastructure.Persistance.Interceptors
{
    // Infrastructure/Interceptors/AuditInterceptor.cs
    public class AuditInterceptor : SaveChangesInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditInterceptor(IHttpContextAccessor accessor)
            => _httpContextAccessor = accessor;

        private static readonly Dictionary<string, string> _entityNameMap = new()
        {
            { "ApplicationUser", "User" },
            { "EmployeeOnBoardingInvitation", "Employee Invitation" },
            { "TaskItem", "Task" },
            { "Outbox", "Email" },
            { "TaskItemHistory", "Task Hisory" },
            { "KPIIncedint", "KPI Incedint" },
            { "SalesLead", "Lead" },
            { "LeadNote", "Lead Note" },
        };

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
        {
            if (eventData.Context is null) return await base.SavingChangesAsync(eventData, result, ct);

            var userEmail = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
            var entries = eventData.Context.ChangeTracker.Entries()
                .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
                .ToList();

            foreach (var entry in entries)
            {
                var rawName = entry.Entity.GetType().Name;
                var entityName = _entityNameMap.GetValueOrDefault(rawName, rawName);

                Dictionary<string, object?>? oldValues = null;
                Dictionary<string, object?>? newValues = null;

                if (entry.State == EntityState.Modified)
                {
                    var changedProps = entry.Properties
                        .Where(p => !Equals(p.OriginalValue, p.CurrentValue))
                        .ToList();
                    //var changedProps = entry.Properties.Where(p => p.IsModified).ToList();

                    oldValues = changedProps.ToDictionary(
                        p => p.Metadata.Name,
                        p => p.OriginalValue);

                    newValues = changedProps.ToDictionary(
                        p => p.Metadata.Name,
                        p => p.CurrentValue);
                }
                else if (entry.State == EntityState.Added)
                {
                    newValues = entry.Properties.ToDictionary(
                        p => p.Metadata.Name,
                        p => p.CurrentValue);
                }
                else if (entry.State == EntityState.Deleted)
                {
                    oldValues = entry.Properties.ToDictionary(
                        p => p.Metadata.Name,
                        p => p.OriginalValue);
                }

                //var log = new AuditLog
                //{
                //    EntityName = entry.Entity.GetType().Name,
                //    Action = entry.State.ToString(), // "Added", "Modified", "Deleted"
                //    EntityId = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey())?.CurrentValue?.ToString(),
                //    OldValues = entry.State == EntityState.Modified || entry.State == EntityState.Deleted
                //        ? JsonSerializer.Serialize(entry.OriginalValues.ToObject())
                //        : null,

                //    NewValues = entry.State == EntityState.Added || entry.State == EntityState.Modified
                //        ? JsonSerializer.Serialize(entry.CurrentValues.ToObject())
                //        : null,
                //    PerformedBy = userId,
                //    CreatedAt = DateTime.UtcNow
                //};

                var log = new AuditLog
                {
                    EntityName = entityName,
                    Action = entry.State.ToString(),
                    EntityId = entry.Properties
                    .FirstOrDefault(p => p.Metadata.IsPrimaryKey())?.CurrentValue?.ToString(),
                    OldValues = oldValues is not null ? JsonSerializer.Serialize(oldValues) : null,
                    NewValues = newValues is not null ? JsonSerializer.Serialize(newValues) : null,
                    PerformedBy = userEmail ?? "System",
                    CreatedAt = DateTime.UtcNow
                };

                eventData.Context.Set<AuditLog>().Add(log);
            }

            return await base.SavingChangesAsync(eventData, result, ct);
        }
    }
}
