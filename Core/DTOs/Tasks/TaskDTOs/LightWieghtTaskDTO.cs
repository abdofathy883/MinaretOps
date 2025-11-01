using Core.DTOs.Tasks.CommentDTOs;
using Core.DTOs.Tasks.TaskResourcesDTOs;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Tasks.TaskDTOs
{
    public class LightWieghtTaskDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required TaskType TaskType { get; set; }
        public string? Description { get; set; }
        public CustomTaskStatus Status { get; set; } = CustomTaskStatus.Open;
        public DateTime Deadline { get; set; }
        public string Priority { get; set; } = "عادي";
        public DateTime? CompletedAt { get; set; }
        public bool IsCompletedOnDeadline =>
            Status == CustomTaskStatus.Completed &&
            CompletedAt.HasValue &&
            CompletedAt.Value <= Deadline;
        public string? EmployeeName { get; set; }
        public string? ServiceName { get; set; }
        public string? ClientName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
