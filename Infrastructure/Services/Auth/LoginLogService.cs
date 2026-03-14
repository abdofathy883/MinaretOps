using Core.DTOs.AuthDTOs;
using Core.Interfaces.Auth;
using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Auth
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
    }
}
