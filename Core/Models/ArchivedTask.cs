using Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("ArchivedTask", Schema = "Tasks")]
    public class ArchivedTask
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
        public ApplicationUser? Employee { get; set; }
        public int? NumberOfSubTasks { get; set; }

        public DateTime? CompletedAt { get; set; }
        public bool IsCompletedOnDeadline =>
            Status == CustomTaskStatus.Completed &&
            CompletedAt.HasValue &&
            CompletedAt.Value <= Deadline;
        public int? TaskGroupId { get; set; }
        public TaskGroup? TaskGroup { get; set; }
        public List<TaskItemHistory> TaskHistory { get; set; } = new();
        public List<TaskCompletionResources> CompletionResources { get; set; } = new();
        public string? CompletionNotes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
