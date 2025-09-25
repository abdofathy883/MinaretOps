using Core.DTOs.Attendance;
using Core.Enums;

namespace Core.Interfaces
{
    public interface IAttendanceService
    {
        Task<AttendanceRecordDTO> NewAttendanceRecord(CreateAttendanceRecordDTO recordDTO);
        Task<List<AttendanceRecordDTO>> GetAttendanceRecordsByEmployee(string employeeId);
        Task<List<AttendanceRecordDTO>> GetAllAttendanceRecords();
        Task<AttendanceRecordDTO> ChangeAttendanceStatusByAdminAsync(string adminId, int recordId, AttendanceStatus newStatus);
        Task<AttendanceRecordDTO> GetTodayAttendanceForEmployeeAsync(string empId);

        Task MarkAbsenteesAsync();
    }
}
