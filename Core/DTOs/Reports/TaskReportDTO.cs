using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Reports
{
    public class TaskReportDTO
    {
        public int TaskId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime Deadline { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsCompletedOnDeadline { get; set; }
        public CustomTaskStatus Status { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string? Refrence { get; set; }
    }
}
