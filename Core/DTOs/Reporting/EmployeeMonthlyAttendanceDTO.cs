using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Reporting
{
    public class EmployeeMonthlyAttendanceDTO
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public int TotalDaysPresent { get; set; }
        public int TotalDaysAbsent { get; set; }
        public int TotalDaysOnLeave { get; set; }
        public double TotalHoursWorked { get; set; }
    }
}
