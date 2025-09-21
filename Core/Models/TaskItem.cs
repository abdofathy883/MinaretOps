using Core.Enums;

namespace Core.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required TaskType TaskType { get; set; }
        public string? Description { get; set; }
        public CustomTaskStatus Status { get; set; } = CustomTaskStatus.Open;
        public int ClientServiceId { get; set; }
        public ClientService ClientService { get; set; } = default!;

        public DateTime Deadline { get; set; }
        public string Priority { get; set; } = "عادي"; // Default priority
        public string? Refrence { get; set; }
        public string? EmployeeId { get; set; }
        public ApplicationUser Employee { get; set; } = default!;

        public DateTime? CompletedAt { get; set; }
        public bool IsCompletedOnDeadline =>
            Status == CustomTaskStatus.Completed &&
            CompletedAt.HasValue &&
            CompletedAt.Value.Date <= Deadline.Date;
        public int TaskGroupId { get; set; }
        public TaskGroup TaskGroup { get; set; } = default!;
    }
}
