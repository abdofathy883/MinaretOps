using Core.Enums;

namespace Core.Models
{
    public class AttendanceRecord
    {
        public int Id { get; set; }
        public required string EmployeeId { get; set; }
        public ApplicationUser Employee { get; set; } = default!;
        public DateTime CheckInTime { get; set; }
        public AttendanceStatus Status { get; set; }
        public required string DeviceId { get; set; }
        public required string IpAddress { get; set; }
    }
}
