using Core.DTOs.AuthDTOs;
using Core.Models;

namespace Core.Interfaces.Auth
{
    public interface ILoginLogService
    {
        Task LogAsync(LoginLog log);
        Task<List<LoginLogDto>> GetAllLogsAsync();
    }
}
