using Core.Enums;

namespace Core.Models
{
    public class AttendanceRecord
    {
        public int Id { get; set; }
        public required string EmployeeId { get; set; }
        public ApplicationUser Employee { get; set; } = default!;
        public DateTime ClockIn { get; set; }
        public DateTime? ClockOut { get; set; }
        public DateOnly WorkDate { get; set; }
        public TimeSpan? TotalWorkingTime => 
            ClockOut.HasValue ? ClockOut.Value - ClockIn : null;
        public bool? MissingClockOut { get; set; }
        public AttendanceStatus Status { get; set; }
        public required string DeviceId { get; set; }
        public required string IpAddress { get; set; }
        public bool EarlyLeave { get; set; } = false;
        public List<BreakPeriod> BreakPeriods { get; set; } = new();
        public TimeSpan TotalBreakTime => TimeSpan.FromMinutes(
        BreakPeriods.Where(b => b.EndTime.HasValue)
              .Sum(b => (b.EndTime.Value - b.StartTime).TotalMinutes)
    );
    }
}
