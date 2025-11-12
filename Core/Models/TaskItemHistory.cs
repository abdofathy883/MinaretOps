namespace Core.Models
{
    public class TaskItemHistory
    {
        public int Id { get; set; }
        public int? TaskItemId { get; set; }  // Changed from int to int? (nullable)
        public TaskItem? TaskItem { get; set; }  // Changed from default! to nullable

        public int? ArchivedTaskId { get; set; }  // ADD THIS
        public ArchivedTask? ArchivedTask { get; set; }  // ADD THIS

        public string PropertyName { get; set; } = default!; // e.g. "Status", "Deadline"
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }

        public string? UpdatedById { get; set; }
        public required string UpdatedByName { get; set; }
        public ApplicationUser UpdatedBy { get; set; } = default!;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
