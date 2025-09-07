using Core.Enums;

namespace Core.DTOs.Attendance
{
    public class CreateLeaveRequestDTO
    {
        public required string EmployeeId { get; set; }
        public DateTime Date { get; set; }
    }
}
