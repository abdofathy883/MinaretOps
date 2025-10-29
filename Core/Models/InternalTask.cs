using Core.Enums;

namespace Core.Models
{
    public class InternalTask
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public required InternalTaskType TaskType { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public CustomTaskStatus Status { get; set; }
        public string Priority { get; set; }
        public bool IsArchived { get; set; }

        public bool IsCompletedOnDeadline =>
            Status == CustomTaskStatus.Completed &&
            CompletedAt.HasValue &&
            CompletedAt.Value <= Deadline;
        public List<InternalTaskAssignment> Assignments { get; set; } = new();
    }
}
