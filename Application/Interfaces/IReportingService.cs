using Application.DTOs.Reporting;
using Core.Enums.Auth_Attendance;

namespace Application.Interfaces
{
    public interface IReportingService
    {
        Task<TaskEmployeeReportDTO> GetTaskEmployeeReportAsync(string currentUserId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<MonthlyAttendanceReportDTO> GetMonthlyAttendanceReportAsync(DateTime fromDate, DateTime toDate, AttendanceStatus? status = null);
    }
}
