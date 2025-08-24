using Core.Enums;

namespace Core.DTOs.Tasks
{
    public class TaskDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public CustomTaskStatus Status { get; set; } = CustomTaskStatus.Open;
        public int ClientServiceId { get; set; }
        public DateTime Deadline { get; set; }
        public string Priority { get; set; } = "عادي"; // Default priority
        public string? Refrence { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsCompletedOnDeadline =>
            Status == CustomTaskStatus.Completed &&
            CompletedAt.HasValue &&
            CompletedAt.Value.Date <= Deadline.Date;
        public int TaskGroupId { get; set; }

        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string ClientName { get; set; }
    }
}
