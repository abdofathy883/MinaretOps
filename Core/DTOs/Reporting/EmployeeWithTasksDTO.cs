using Core.DTOs.Tasks.TaskDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Reporting
{
    public class EmployeeWithTasksDTO
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public bool? IsOnBreak { get; set; }
        public DateTime? ClockInTime { get; set; }
        public TimeSpan? WorkingDuration { get; set; }
        public List<LightWieghtTaskDTO> Tasks { get; set; } = new();
    }
}
