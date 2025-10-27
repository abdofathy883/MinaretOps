using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Attendance.AttendanceDashboard
{
    public class ActiveEmployeeDTO
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public bool IsOnBreak { get; set; }
        public DateTime? ClockInTime { get; set; }
        public TimeSpan? WorkingDuration { get; set; }
    }
}
