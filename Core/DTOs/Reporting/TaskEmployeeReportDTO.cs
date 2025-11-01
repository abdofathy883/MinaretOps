using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Reporting
{
    public class TaskEmployeeReportDTO
    {
        public List<EmployeeWithTasksDTO> WorkingEmployees { get; set; } = new();
        public List<EmployeeWithTasksDTO> OnBreakEmployees { get; set; } = new();
        public List<EmployeeWithTasksDTO> AbsentEmployees { get; set; } = new();
    }
}
