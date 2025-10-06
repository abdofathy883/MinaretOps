using Core.Enums;

namespace Core.DTOs.Attendance
{
    public class AttendanceRecordDTO
    {
        public int Id { get; set; }
        public required string EmployeeId { get; set; }
        public required string EmployeeName { get; set; }
        public DateTime ClockIn { get; set; }
        public DateTime ClockOut { get; set; }
        public AttendanceStatus Status { get; set; }
        public required string DeviceId { get; set; }
        public required string IpAddress { get; set; }
    }
}
