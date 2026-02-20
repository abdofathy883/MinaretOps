using Core.Enums.Auth_Attendance;
using Microsoft.AspNetCore.Http;

namespace Core.DTOs.Attendance
{
    public class CreateLeaveRequestDTO
    {
        public required string EmployeeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public LeaveRequestType Type { get; set; }
        public required string Reason { get; set; }
        public IFormFile? ProofFile { get; set; }
    }
}
