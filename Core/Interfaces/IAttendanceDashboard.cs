using Core.DTOs.Attendance.AttendanceDashboard;

namespace Core.Interfaces
{
    public interface IAttendanceDashboard
    {
        Task<AttendanceDashboardDTO> GetAttendanceDashboardAsync();
    }
}
