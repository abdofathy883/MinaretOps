using Application.DTOs.Attendance.AttendanceDashboard;

namespace Application.Interfaces
{
    public interface IAttendanceDashboard
    {
        Task<AttendanceDashboardDTO> GetAttendanceDashboardAsync();
    }
}
