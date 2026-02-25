using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("BreakPeriod", Schema = "HR")]
    public class BreakPeriod
    {
        public int Id { get; set; }

        public int AttendanceRecordId { get; set; }
        public AttendanceRecord AttendanceRecord { get; set; } = default!;

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public TimeSpan? Duration => EndTime.HasValue
            ? EndTime.Value - StartTime
            : null;
    }
}
