using Core.DTOs.Attendance;
using Core.Enums.Auth_Attendance;

namespace Core.Interfaces
{
    public interface ILeaveRequestService
    {
        Task<LeaveRequestDTO> SubmitLeaveRequest(CreateLeaveRequestDTO leaveRequestDTO);
        Task<List<LeaveRequestDTO>> GetLeaveRequestsByEmployee(string employeeId);
        Task<List<LeaveRequestDTO>> GetAllLeaveRequests();
        Task<LeaveRequestDTO> ChangeLeaveRequestStatusByAdminAsync(string adminId, int requestId, LeaveStatus newStatus);

    }
}
