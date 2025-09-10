using Core.Enums;

namespace Core.DTOs.Attendance
{
    public class CreateLeaveRequestDTO
    {
        public required string EmployeeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
