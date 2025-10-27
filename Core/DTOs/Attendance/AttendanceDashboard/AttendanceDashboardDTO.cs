using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Attendance.AttendanceDashboard
{
    public class AttendanceDashboardDTO
    {
        // Currently Active Column
        public int CurrentlyActiveTotal { get; set; }
        public List<ActiveEmployeeDTO> CurrentlyActiveEmployees { get; set; } = new();

        // Absent Column
        public int AbsentTotal { get; set; }
        public List<EmployeeDTO> AbsentEmployees { get; set; } = new();

        // On Leave Column
        public int OnLeaveTotal { get; set; }
        public List<EmployeeDTO> OnLeaveEmployees { get; set; } = new();
    }
}
