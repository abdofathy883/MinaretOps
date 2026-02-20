using Core.DTOs.AttendanceBreaks;
using Core.Enums.Auth_Attendance;

namespace Core.DTOs.Attendance
{
    public class AttendanceRecordDTO
    {
        public int Id { get; set; }
        public required string EmployeeId { get; set; }
        public required string EmployeeName { get; set; }
        public DateTime ClockIn { get; set; }
        public bool IsClockedInAfterSchedule { get; set; }
        public DateTime? ClockOut { get; set; }
        public DateOnly WorkDate { get; set; }
        public AttendanceStatus Status { get; set; }
        public bool? MissingClockOut { get; set; }
        public bool EarlyLeave { get; set; }
        public List<BreakDTO> Breaks { get; set; } = new();
        public TimeSpan? TotalWorkingTime { get; set; }
        public TimeSpan TotalBreakTime { get; set; }
    }
}
