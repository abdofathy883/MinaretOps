using Core.Enums;
using Core.Models;

namespace Core.DTOs.Tasks
{
    public class CreateTaskDTO
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public CustomTaskStatus Status { get; set; }
        public int ClientServiceId { get; set; }

        public DateTime Deadline { get; set; }
        public string Priority { get; set; } = "عادي"; // Default priority
        public string? Refrence { get; set; }
        public string EmployeeId { get; set; }

        public DateTime? CompletedAt { get; set; }
        public bool IsCompletedOnDeadline =>
            Status == CustomTaskStatus.Completed &&
            CompletedAt.HasValue &&
            CompletedAt.Value.Date <= Deadline.Date;
        public int TaskGroupId { get; set; }
    }
}
