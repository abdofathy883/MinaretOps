using Application.DTOs.AuditLogs;
using Application.DTOs.AuthDTOs;
using Application.Interfaces.Auth;
using Core.Models;
using DocumentFormat.OpenXml.InkML;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity
{
    public class LoginLogService : ILoginLogService
    {
        private readonly MinaretOpsDbContext _dbContext;

        public LoginLogService(MinaretOpsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<LoginLogDto>> GetAllLogsAsync()
        {
            var logs = await _dbContext.LoginLogs
                .Select(log => new LoginLogDto
                {
                    Id = log.Id,
                    UserId = log.UserId,
                    UserName = log.User.FirstName + " " + log.User.LastName,
                    Timestamp = log.Timestamp,
                    IpAddress = log.IpAddress,
                    IsSuccess = log.IsSuccess,
                    FailureReason = log.FailureReason,
                    UserAgent = log.UserAgent
                })
                .ToListAsync();
            return logs;
        }

        public async Task LogAsync(LoginLog log)
        {
            await _dbContext.LoginLogs.AddAsync(log);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<PagedResult<AuditLogDto>> GetAuditLogsAsync(string? entityName, int page, int pageSize)
        {
            var query = _dbContext.AuditLogs.AsNoTracking();

            if (!string.IsNullOrEmpty(entityName))
                query = query.Where(x => x.EntityName == entityName);

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new AuditLogDto(
                    x.Id, x.EntityName, x.Action,
                    x.EntityId, x.OldValues, x.NewValues,
                    x.PerformedBy, x.CreatedAt))
                .ToListAsync();

            return new PagedResult<AuditLogDto>(items, total, page, pageSize);
        }
    }
}
