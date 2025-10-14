using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
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
