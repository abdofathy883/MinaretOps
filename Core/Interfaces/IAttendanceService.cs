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

        // Leave Requests
        Task<LeaveRequestDTO> SubmitLeaveRequest(CreateLeaveRequestDTO leaveRequestDTO);
        Task<List<LeaveRequestDTO>> GetLeaveRequestsByEmployee(string employeeId);
        Task<List<LeaveRequestDTO>> GetAllLeaveRequests();
        Task<LeaveRequestDTO> ChangeLeaveRequestStatusByAdminAsync(string adminId, int requestId, LeaveStatus newStatus);

    }
}
