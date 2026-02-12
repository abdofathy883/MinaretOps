using Core.DTOs.AttendanceBreaks;

namespace Core.Interfaces
{
    public interface IBreakService
    {
        Task<BreakDTO> StartBreakAsync(string currentUserId);
        Task<BreakDTO> EndBreakAsync(string currentUserId);
        Task<BreakDTO?> GetActiveBreakAsync(string employeeId);
    }
}
