using Core.DTOs.Attendance;
using Core.Enums;

namespace Core.Interfaces
{
    public interface IAttendanceService
    {
        Task<AttendanceRecordDTO> ClockInAsync(CreateAttendanceRecordDTO recordDTO);
        Task<AttendanceRecordDTO> ClockOutAsync(CreateAttendanceRecordDTO recordDTO);
        Task<List<AttendanceRecordDTO>> GetAllAttendanceRecords();
        Task<AttendanceRecordDTO> ChangeAttendanceStatusByAdminAsync(string adminId, int recordId, AttendanceStatus newStatus);
        Task<AttendanceRecordDTO> GetTodayAttendanceForEmployeeAsync(string empId);

        Task MarkAbsenteesAsync();
        //Task<List<AttendanceRecordDTO>> GetMonthlyStatusForEmpAsync();
    }
}
