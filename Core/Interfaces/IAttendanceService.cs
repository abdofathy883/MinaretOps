using Core.DTOs.Attendance;
using Core.Enums;

namespace Core.Interfaces
{
    public interface IAttendanceService
    {
        Task<AttendanceRecordDTO> ClockInAsync(CreateAttendanceRecordDTO recordDTO);
        Task<AttendanceRecordDTO> ClockOutAsync(string empId);
        Task<List<AttendanceRecordDTO>> GetAllAttendanceRecords(DateOnly date);
        Task<AttendanceRecordDTO> ChangeAttendanceStatusByAdminAsync(string adminId, int recordId, AttendanceStatus newStatus);
        Task<AttendanceRecordDTO> GetTodayAttendanceForEmployeeAsync(string empId);
        Task<PaginatedAttendanceResultDTO> GetAttendanceRecordsAsync(AttendanceFilterDTO filter);
        Task<bool> SubmitEarlyLeaveByEmpIdAsync(ToggleEarlyLeaveDTO earlyLeave);
        Task MarkAbsenteesAsync();
        //Task<List<AttendanceRecordDTO>> GetMonthlyReportForEmpAsync();
    }
}
