using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Reporting
{
    public class MonthlyAttendanceReportDTO
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<EmployeeMonthlyAttendanceDTO> Employees { get; set; } = new();
    }
}
