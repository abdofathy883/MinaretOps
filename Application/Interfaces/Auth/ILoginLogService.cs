using Application.DTOs.AuditLogs;
using Application.DTOs.AuthDTOs;
using Core.Models;

namespace Application.Interfaces.Auth
{
    public interface ILoginLogService
    {
        Task LogAsync(LoginLog log);
        Task<List<LoginLogDto>> GetAllLogsAsync();
        Task<PagedResult<AuditLogDto>> GetAuditLogsAsync(string? entityName, int page, int pageSize);

    }
}
