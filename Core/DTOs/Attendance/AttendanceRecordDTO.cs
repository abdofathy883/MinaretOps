using Core.DTOs.AttendanceBreaks;
using Core.Enums;

namespace Core.DTOs.Attendance
{
    public class AttendanceRecordDTO
    {
        public int Id { get; set; }
        public required string EmployeeId { get; set; }
        public required string EmployeeName { get; set; }
        public DateTime ClockIn { get; set; }
        public DateTime? ClockOut { get; set; }
        public AttendanceStatus Status { get; set; }
        public List<BreakDTO> Breaks { get; set; } = new();
        public TimeSpan? TotalWorkingTime { get; set; }
        public TimeSpan TotalBreakTime { get; set; }
    }
}
