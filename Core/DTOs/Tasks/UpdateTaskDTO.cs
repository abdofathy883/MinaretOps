using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Tasks
{
    public class UpdateTaskDTO
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? Deadline { get; set; }
        public CustomTaskStatus Status { get; set; }
        public string? Priority { get; set; }
        public string? Refrence { get; set; }
        public string? EmployeeId { get; set; }
    }
}
